namespace WCS.Core
{
    public struct WCSRequestHeaderData
	{
		public const string DeviceName = "X-WCS-DeviceName";
		public const string ApplicationVersion = "X-WCS-ApplicationVersion";
		public const string OSHeader = "X-WCS-OS";
		public const string LocationHeader = "X-WCS-Location";
		
		public const string PrivateMemorySizeHeader = "X-WCS-PrivateMemorySize";

		public string Device { get; set; }
		public string OS { get; set; }
		public string Version { get; set; }
		public string Location { get; set; }
		public string IPAddress { get; set; }
	}
}