using System;

namespace BrowseTheWeb
{
	public class CommandContainer<T>
	{
		Action<T> action;
		public CommandContainer(Action<T> action)
		{
			this.action = action;
		}
		public void Execute(T item)
		{
			action(item);
		}
	}

    public class CommandContainer
    {
        Action action;
        public CommandContainer(Action action)
        {
            this.action = action;
        }
        public void Execute()
        {
            action();
        }
    }
   
}
