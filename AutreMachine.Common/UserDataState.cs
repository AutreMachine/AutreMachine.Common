
namespace AutreMachine.Common
{
    public enum TypeTutorial
    {
        Main=1,
        MoneyHistory=2,
        Bets=4,
        MySituation_Financial=8,
        MySituation_Assets=16,
        Buy_Stock=32,
        Buy_Crypto=64,
        Buy_Commodity=128,
        Buy_Currency=256,
        HallOfFame=512,
        StopLossTakeProfit=1024
    }

    /// <summary>
    /// This class handles data that are shared through different components and can evolve through app lifetime.
    /// Ex : NbUnreadMessages : I see the badge top right, it decreases when I read a New message
    /// </summary>
    public class UserDataState
    {
        /*
        /// <summary>
        /// For MessagesDropDown
        /// </summary>
        public int NbUnreadMessages { get; private set; }
        /// <summary>
        /// For Profile
        /// </summary>
        public string? UrlProfilePic { get; private set; }
        public string? DisplayName { get; private set; }
        /// <summary>
        /// For AlertBet
        /// </summary>
        public MessageUnitViewModel? BetWinnerMessage { get; private set; }
        public MessageUnitViewModel? BetLoserMessage { get; private set; }
        
        public TypeTutorial? TypeTutorial { get; private set; }

        public event Action? OnNbUnreadMessagesChange;
        public event Action? OnProfilePicChange;
        public event Action? OnDisplayNameChange;
        /// <summary>
        /// When the user wins a bet
        /// </summary>
        public event Action? OnBetWinnerChange;
        /// <summary>
        /// When the user loses a bet
        /// </summary>
        public event Action? OnBetLoserChange;
        public event Action? OnBetHideAlert;
        /// <summary>
        /// Tutorials
        /// </summary>
        public event Action? OnShowTutorial;
        //public event Action? OnTypeTutorialChange;

        public void SetNbUnreadMessages(int nbUnreadMessages)
        {
            NbUnreadMessages = nbUnreadMessages;
            NotifyNbUnreadStateChanged();            
        }

        public void SetUrlProfilePic(string urlProfilePic)
        {
            UrlProfilePic = urlProfilePic;
            NotifyProfilePicStateChanged();
        }
        public void SetDisplayName(string displayName)
        {
            DisplayName = displayName;
            NotifyDisplayNameStateChanged();
        }

        public void SetBetWinner(MessageUnitViewModel message)
        {
            BetWinnerMessage = message;
            NotifyBetWinnerStateChanged();
        }
        public void SetBetLoser(MessageUnitViewModel message)
        {
            BetLoserMessage = message;
            NotifyBetLoserStateChanged();
        }

        public void SetTypeTutorialToShow(TypeTutorial typeTutorial)
        {
            TypeTutorial = typeTutorial;
            NotifyTutorialToShowStateChanged();
        }

        public void HideAlertBet()
        {
            NotifyHideAlertBet();
        }


        private void NotifyNbUnreadStateChanged() => OnNbUnreadMessagesChange?.Invoke();
        
        private void NotifyProfilePicStateChanged() => OnProfilePicChange?.Invoke();
        private void NotifyDisplayNameStateChanged() => OnDisplayNameChange?.Invoke();
        
        private void NotifyBetWinnerStateChanged() => OnBetWinnerChange?.Invoke();
        private void NotifyBetLoserStateChanged() => OnBetLoserChange?.Invoke();

        //private void NotifyShowTutorial() => OnShowTutorial?.Invoke();
        private void NotifyTutorialToShowStateChanged() => OnShowTutorial?.Invoke();
        private void NotifyHideAlertBet() => OnBetHideAlert?.Invoke();
        */
    }
}
