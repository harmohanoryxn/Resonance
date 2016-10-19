using System.Media;

namespace WCS.Core
{
	public static class SoundsExtensions
	{
		public static void Play(this Sound sound)
		{
			switch (sound)
			{
				case Sound.Error:
					PlaySound("Error.wav");
					break;
				case Sound.Submit:
					PlaySound("ButtonSubmit.wav");
					break;
				case Sound.OpenPeel:
					PlaySound("PeelOpen.wav");
					break;
				case Sound.UpdateRecieved:
					PlaySound("UpdateReceived.wav");
					break;
				case Sound.UpdateSent:
					PlaySound("UpdateSent.wav");
					break;
				case Sound.ButtonClick:
					PlaySound("ButtonClick.wav");
					break;
				case Sound.ButtonOver:
					PlaySound("ButtonOver.wav");
					break;
			
			}

		}

		private static void PlaySound(string filename)
		{
			//Uri uri = new Uri(@"pack://application:,,,/WCS.Galway;Component/Media/Sounds/Error.wav");
			//SoundPlayer player = new SoundPlayer(Application.GetResourceStream(uri).Stream);
			//player.Play();
			 
			var sp = new SoundPlayer(string.Format(@"Media/Sounds/{0}", filename));
			sp.Play();
		}
	}
}
