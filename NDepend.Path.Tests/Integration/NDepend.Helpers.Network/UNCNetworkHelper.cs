using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using NDepend.Helpers.Interop;
using NDepend.Path;


//
// 20Jan2014: Code extracted from https://code.google.com/p/lanexchange/
//            and refactored for our needs!
//
namespace NDepend.Helpers.Network {
   internal static class UNCNetworkHelper {


      internal static IAbsoluteDirectoryPath[] GetExistingUNCShares() {
         try {
            var list = new List<IAbsoluteDirectoryPath>();
            foreach (var item in GetNetServers("WORKGROUP", Win32MethodsNetwork.SV_101_TYPES.SV_TYPE_ALL)) {
               var serverInfo = ServerInfo.FromNetApi32(item);

               foreach (var itemShare in GetNetShares(serverInfo.Name)) {
                  var shareInfo = new ShareInfo(itemShare);
                  if (shareInfo.IsHidden || shareInfo.IsPrinter) {
                     continue;
                  }
                  list.Add((@"\\" + serverInfo.Name + @"\" + shareInfo.Name).ToAbsoluteDirectoryPath());
               }
            }
            return list.ToArray();
         }
         catch {
            // 20Jan2014: so far didn't happen, but don't trust too much this code
            return new IAbsoluteDirectoryPath[0];
         }
      }



      private static IEnumerable<Win32MethodsNetwork.SERVER_INFO_101> GetNetServers(string domain, Win32MethodsNetwork.SV_101_TYPES types) {
         IntPtr pInfo;
         uint entriesread = 0;
         uint totalentries = 0;
         Win32MethodsNetwork.NERR err;
         unchecked {
            err = Win32MethodsNetwork.NetServerEnum(null, 101, out pInfo, (uint)-1, ref entriesread, ref totalentries, types, domain, 0);
         }
         if ((err == Win32MethodsNetwork.NERR.NERR_SUCCESS ||
              err == Win32MethodsNetwork.NERR.ERROR_MORE_DATA) &&
             pInfo != IntPtr.Zero) {
            try {
               int ptr = pInfo.ToInt32();
               for (int i = 0; i < entriesread; i++) {
                  yield return (Win32MethodsNetwork.SERVER_INFO_101) Marshal.PtrToStructure(new IntPtr(ptr), typeof(Win32MethodsNetwork.SERVER_INFO_101));
                  ptr += Marshal.SizeOf(typeof(Win32MethodsNetwork.SERVER_INFO_101));
               }
            } finally {
               if (pInfo != IntPtr.Zero) {
                  Win32MethodsNetwork.NetApiBufferFree(pInfo);
               }
            }
         }
      }

      private static IEnumerable<Win32MethodsNetwork.SHARE_INFO_1> GetNetShares(string computer) {
         IntPtr pInfo;
         int entriesread = 0;
         int totalentries = 0;
         Win32MethodsNetwork.NERR err = Win32MethodsNetwork.NetShareEnum(computer, 1, out pInfo, Win32MethodsNetwork.MAX_PREFERRED_LENGTH, ref entriesread, ref totalentries, 0);
         if ((err == Win32MethodsNetwork.NERR.NERR_SUCCESS ||
              err == Win32MethodsNetwork.NERR.ERROR_MORE_DATA) &&
             pInfo != IntPtr.Zero) {
            try {
               const uint stypeIPC = (uint)Win32MethodsNetwork.SHARE_TYPE.STYPE_IPC;
               int ptr = pInfo.ToInt32();
               for (int i = 0; i < entriesread; i++) {
                  var shi1 = (Win32MethodsNetwork.SHARE_INFO_1) Marshal.PtrToStructure(new IntPtr(ptr), typeof(Win32MethodsNetwork.SHARE_INFO_1));
                  if ((shi1.shi1_type & stypeIPC) != stypeIPC) {
                     yield return shi1;
                  }
                  ptr += Marshal.SizeOf(typeof(Win32MethodsNetwork.SHARE_INFO_1));
               }
            } finally {
               if (pInfo != IntPtr.Zero) {
                  Win32MethodsNetwork.NetApiBufferFree(pInfo);
               }
            }
         }
      }

      private sealed class ServerInfo {
         internal static ServerInfo FromNetApi32(Win32MethodsNetwork.SERVER_INFO_101 info) {
            var result = new ServerInfo(info.sv101_name);
            return result;
         }

         private ServerInfo(string name) { m_Name = name; }

         private readonly string m_Name;
         internal string Name { get { return m_Name; } }
      }


      private sealed class ShareInfo {

         internal ShareInfo(Win32MethodsNetwork.SHARE_INFO_1 info) {
            m_Info = info;
         }
         private readonly Win32MethodsNetwork.SHARE_INFO_1 m_Info;

         internal string Name { get { return m_Info.shi1_netname; } }

         internal bool IsPrinter {
            get { return (m_Info.shi1_type == (uint)Win32MethodsNetwork.SHARE_TYPE.STYPE_PRINTQ); }
         }

         internal bool IsHidden {
            get {
               if (String.IsNullOrEmpty(m_Info.shi1_netname)) {
                  return false;
               }
               return m_Info.shi1_netname[m_Info.shi1_netname.Length - 1] == '$';
            }
         }
      }
   }
}