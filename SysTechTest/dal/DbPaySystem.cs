namespace SysTechTest.dal
{
    public class PaySystemDesc : DbDictionary { }
    public class PayParamBaseModel : BaseModel
    {
        private bool m_isEnabled = false;
        private PaySystemDesc m_paySystemDesc;
        public PayParamBaseModel(DbHelpers.PaySystem paySystemDesc) {
            PaySystemDescId = (int)paySystemDesc;
        }
        public int Id { get; set; }
        public int PaySystemDescId { get; set; }
        public int GroupId { get; set; }
        public bool IsEnabled {
            get { return m_isEnabled; }
            set { m_isEnabled = value; OnPropertyChanged(); }
        }
        public void SetPaySystemDesc(PaySystemDesc desc) => m_paySystemDesc = desc;
        public PaySystemDesc GetPaySystemDescOrNull() => m_paySystemDesc;
    }
    public class PayBaseRateParam : PayParamBaseModel
    {
        private decimal m_baseRate = decimal.Zero;
        public PayBaseRateParam() : base(DbHelpers.PaySystem.PayBaseRate) { }
        public decimal BaseRate {
            get { return m_baseRate; }
            set { m_baseRate = value; OnPropertyChanged(); }
        }
    }
    public class PayExperienceParam : PayParamBaseModel
    {
        private decimal m_percentInAYear = decimal.Zero;
        private decimal m_maxPercent = decimal.Zero;
        public PayExperienceParam() : base(DbHelpers.PaySystem.PayExperience) { }

        public decimal PercentInAYear {
            get { return m_percentInAYear; }
            set { m_percentInAYear = value; OnPropertyChanged(); }
        }
        public decimal MaxPercent {
            get { return m_maxPercent; }
            set { m_maxPercent = value; OnPropertyChanged(); }
        }
    }
    public class PayForSubordinatesParam : PayParamBaseModel
    {
        private bool m_onlyFirstLevelEnabled = true;
        private decimal m_percent = decimal.Zero;
        public PayForSubordinatesParam() : base(DbHelpers.PaySystem.PayForSubordinates) { }

        public bool OnlyFirstLevelEnabled {
            get { return m_onlyFirstLevelEnabled; }
            set { m_onlyFirstLevelEnabled = value; OnPropertyChanged(); }
        }
        public decimal Percent {
            get { return m_percent; }
            set { m_percent = value; OnPropertyChanged(); }
        }
    }
    public class PaySystem
    {
        public PaySystem() {
            PayBaseRateParams = new PayBaseRateParam();
            PayExperienceParams = new PayExperienceParam();
            PayForSubordinatesParams = new PayForSubordinatesParam();
        }
        public PaySystem( PayBaseRateParam p1, PayExperienceParam p2, PayForSubordinatesParam p3) 
        {
            PayBaseRateParams = p1;
            PayExperienceParams = p2;
            PayForSubordinatesParams = p3;
        }
        public PayBaseRateParam PayBaseRateParams { get; private set; }
        public PayExperienceParam PayExperienceParams { get; private set; }
        public PayForSubordinatesParam PayForSubordinatesParams { get; private set; }
    }
}
