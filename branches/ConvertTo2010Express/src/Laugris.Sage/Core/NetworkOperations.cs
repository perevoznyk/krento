
namespace Laugris.Sage
{
    public static class NetworkOperations
    {
        public static bool PortAvailable(int portNumber)
        {
            return NativeMethods.PortAvailable(portNumber);
        }
    }
}
