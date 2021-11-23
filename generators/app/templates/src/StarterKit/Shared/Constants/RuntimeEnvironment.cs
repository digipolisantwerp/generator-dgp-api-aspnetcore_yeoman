namespace StarterKit.Shared.Constants
{
    /// <summary>
    /// additional environments to Microsoft.Extensions.Hosting.Abstractions/EnvironmentName.cs
    /// </summary>
    public static class RuntimeEnvironment
    {
        public static readonly string Local = "Local";
        public static readonly string IntegrationTesting = "IntegrationTesting";
        public static readonly string Development = "Development";
        public static readonly string Acceptance = "Acceptance";
        public static readonly string Staging = "Staging";
        public static readonly string Production = "Production";
    }
}
