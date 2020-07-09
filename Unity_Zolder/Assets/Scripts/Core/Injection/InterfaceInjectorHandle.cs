// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Logging;

namespace Talespin.Core.Foundation.Injection
{
	public struct InterfaceInjectorHandle
	{
		public IInjectorInteractor Context { get; private set; }
		public InjectorInteractorModule Target { get; private set; }

		public InterfaceInjectorHandle(IInjectorInteractor context, InjectorInteractorModule target)
		{
			Context = context;
			Target = target;
		}

		/// <summary>
		/// Injects for context.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="toInject">To inject.</param>
		public void InjectForContext(object context, object toInject)
		{
			if (Target != null)
			{
				Target.InjectForContext(context, toInject);
			}
			else
			{
				LogUtil.Error(LogTags.SYSTEM, this, "Trying to inject for context " + context + " while target is null.");
			}
		}

		public void RemoveFromContext(object context, object toRemove)
		{
			if (Target != null)
			{
				Target.RemoveFromContext(context, toRemove);
			}
			else
			{
				LogUtil.Error(LogTags.SYSTEM, this, "Trying to remove from context " + context + " while target is null.");
			}
		}

		public void ClearContext(object context)
		{
			if (Target != null)
			{
				Target.ClearContext(context);
			}
			else
			{
				LogUtil.Error(LogTags.SYSTEM, this, "Trying to clear context " + context + " while target is null.");
			}
		}

		/// <summary>
		/// Inject an object for the received context. It will automatically be removed when the context is removed.
		/// </summary>
		/// <param name="o">O.</param>
		public void Inject(object o)
		{
			if (Target != null)
			{
				Target.InjectForContext(Context, o);
			}
			else
			{
				LogUtil.Error(LogTags.SYSTEM, this, "Trying to inject into context " + Context + " while target is null.");
			}
		}

		public void Remove(object o)
		{
			if (Target != null)
			{
				Target.RemoveFromContext(Context, o);
			}
			else
			{
				LogUtil.Error(LogTags.SYSTEM, this, "Trying to remove from context " + Context + " while target is null.");
			}
		}

		public string Log()
		{
			return Context.ToString();
		}

		public void Dispose()
		{
			if (Target != null)
			{
				Target.ClearContext(Context);
			}
			Target = null;
			Context = null;
		}
	}
}