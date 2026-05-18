//202322158 이준상


/// <summary>
/// Classes that notify you unconditionally every time they are assigned (set), even if the value is the same as before.
/// It is suitable for event logic such as 'my turn again' of the game.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObservableEvent<T> : Observable<T>
{
    public ObservableEvent(T initialValue = default) : base(initialValue)
    { }
    
    public new T Value
    {
        get => _value; 
        set 
        {
            _value = value;
            Notify(_value);
        }
    }
}