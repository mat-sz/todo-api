namespace TodoAPI.Entities
{
    public class TodoItemLabel
    {
        public int TodoItemId { get; set; }
        public TodoItem TodoItem { get; set; }
        public int LabelId { get; set; }
        public Label Label { get; set; }
    }
}