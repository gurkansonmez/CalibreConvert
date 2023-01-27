namespace CalibreSignal
{
	public class Notification<T>
	{

		public string TransactionId
		{
			get; set;
		}
		public T Object
		{
			get; set;
		}
		List<string> errors = new();
		public List<string> Errors
		{
			get => errors;
		}
	}
}