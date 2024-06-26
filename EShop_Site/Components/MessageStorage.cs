namespace EShop_Site.Components;

public static class MessageStorage
{
    private static string _errorMessage = "";

    public static string ErrorMessage
    {
        get
        {
            string temp = _errorMessage;
            _errorMessage = "";
            return temp;
        }
        set
        {
            if (!value.Equals("") && _errorMessage.Equals(""))
            {
                _errorMessage = value;
            }
        } 
    }
    
    private static string _infoMessage = "";

    public static string InfoMessage
    {
        get
        {
            string temp = _infoMessage;
            _infoMessage = "";
            return temp;
        }
        set
        {
            if (!value.Equals("") && _infoMessage.Equals(""))
            {
                _infoMessage = value;
            }
        } 
    }
    
    private static string _confirmMessage = "";

    public static string ConfirmMessage
    {
        get
        {
            string temp = _confirmMessage;
            _confirmMessage = "";
            return temp;
        }
        set
        {
            if (!value.Equals("") && _confirmMessage.Equals(""))
            {
                _confirmMessage = value;
            }
        } 
    }
    
    private static string _linkMessage = "";

    public static string LinkMessage
    {
        get
        {
            string temp = _linkMessage;
            _linkMessage = "";
            return temp;
        }
        set
        {
            if (!value.Equals("") && _linkMessage.Equals(""))
            {
                _linkMessage = value;
            }
        } 
    }
    
    private static string _findMessage = "";

    public static string FindMessage
    {
        get
        {
            string temp = _findMessage;
            _findMessage = "";
            return temp;
        }
        set
        {
            if (!value.Equals("") && _findMessage.Equals(""))
            {
                _findMessage = value;
            }
        } 
    }
}