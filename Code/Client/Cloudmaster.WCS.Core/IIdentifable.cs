namespace WCS.Core
{
	public interface IIdentifable
	{
		int Id { get; }
		int GetFingerprint();
	}
}
