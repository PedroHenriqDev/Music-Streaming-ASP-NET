namespace ApplicationLayer.ViewModels;

public class RedirectViewModel<T> 
{
    public string RedirectUrl { get; set; }
    public T? Value { get; set; }
}
