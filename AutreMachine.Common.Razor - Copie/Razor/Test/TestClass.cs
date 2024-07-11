


namespace AutreMachine.Common.Razor.Test
{
    public class TestClass : ITableCRUDId
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public virtual TestCategory Category { get; set; } // To fill the displayed value



    }

    public class TestCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
