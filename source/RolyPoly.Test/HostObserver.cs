namespace RolyPoly.Test
{
    public class HostObserver : IObserver<HostInfo>
    {
        public HostObserver()
        {
            Asserts = new Action<HostInfo>((info) => { });
            Completion = new Action(() => { });
            Error = new Action<Exception>((ex) => { });
        }

        public void OnCompleted()
        {
            Completion.Invoke();
        }

        public void OnError(Exception error)
        {
            Error.Invoke(error);
        }

        public void OnNext(HostInfo value)
        {
            Asserts.Invoke(value);
        }

        public Action<HostInfo> Asserts { get; set; }

        public Action Completion { get; set; }

        public Action<Exception> Error { get; set; }
    }
}
