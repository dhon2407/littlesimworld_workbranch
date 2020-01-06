namespace LSW.Tooltip
{
    public interface ITooltip<in T>
    {
        void SetData(T data);
        void Show(T data);
        void Hide();
    }
}