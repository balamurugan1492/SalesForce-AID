namespace Pointel.Softphone.Voice.Core
{
    /// <summary>
    /// Phone function enumerator
    /// </summary>
    internal enum PhoneFunctions
    {
        Intialize,
        Login,
        Logout,
        Ready,
        NotReady,
        Dial,
        Answer,
        Hold,
        Retrieve,
        Release,
        IntiateConference,
        CompleteConference,
        CancelConference,
        InitiateTransfer,
        CompleteTransfer,
        CancelTransfer,
        UpdateUserData,
    }
}