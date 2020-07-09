// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Logging;

namespace Talespin.Core.Foundation.Services
{
	public class ConsoleViewer
	{
#if USE_CONSOLE
		private Queue<string> messages = new Queue<string>();
		private bool show = false;
#endif
		public ConsoleViewer(ICallbackService callbackService)
		{
#if USE_CONSOLE
			callbackService.GUIEvent += OnGUI;
#endif
		}

		public void Log(string msg)
		{
#if USE_CONSOLE
			messages.Enqueue(FrameTimestamp.Create() + ": " + msg);
#else
			LogUtil.Log(LogTags.SYSTEM, this, msg);
#endif
		}

		public void Show()
		{
#if USE_CONSOLE
			show = true;
#endif
		}

		public void Hide()
		{
#if USE_CONSOLE
			show = false;
#endif
		}

#if USE_CONSOLE
		void OnGUI()
		{
			if (GUI.Button(new Rect(0, 0, 18, 18), "x"))
			{
				show = !show;
			}

			if (show)
			{
				string total = "";
				foreach (string s in messages)
				{
					total += s + "\n";
				}
				GUI.TextField(new Rect(18, 0, Screen.width - 18, Screen.height), total); 
			}
		}
#endif
	}
}