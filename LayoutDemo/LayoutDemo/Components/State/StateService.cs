namespace LayoutDemo.Components.State
{
    public class StateService
    {
        public event Action<string, ObjectState> OnStateChanged;

        private int counter = 0;

        public int GetIndex()
        {
            Interlocked.Increment(ref counter);
            return counter;
        }

        public void StateChange(string formName, ObjectState state)
        {
            OnStateChanged?.Invoke(formName, state);
        }
    }
}