

namespace NDepend.Test {
   static class TestHelper {
      private static bool s_TestAlreadySetup = false;
      internal static void SetUpTests() {
         if (s_TestAlreadySetup) { return; }
         // This code let Debug.Assert() msgBox being shown (if violation) at test time.
         s_TestAlreadySetup = true;
         var listener = (System.Diagnostics.DefaultTraceListener)System.Diagnostics.Trace.Listeners[0];
         listener.AssertUiEnabled = true;
      }
   }
}
