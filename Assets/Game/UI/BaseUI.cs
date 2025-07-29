using UnityEngine;

/// <summary>
///     Base class for all user interfaces in the game.
/// </summary>
public abstract class BaseUI : MonoBehaviour {
    public delegate void OnClose(BaseUI ui);

    public delegate void OnOpen(BaseUI ui);

    /// <summary>
    ///     Whether the user interface is open.
    /// </summary>
    public bool IsOpen { get; private set; }

    /// <summary>
    ///     Raised when the user interface is opened.
    /// </summary>
    public event OnOpen Opened;

    /// <summary>
    ///     Raised when the user interface is closed.
    /// </summary>
    public event OnClose Closed;

    /// <summary>
    ///     Open the user interface.
    /// </summary>
    public virtual void Open() {
        IsOpen = true;
        gameObject.SetActive(true);
        Opened?.Invoke(this);
    }

    /// <summary>
    ///     Close the user interface.
    /// </summary>
    public virtual void Close() {
        IsOpen = false;
        gameObject.SetActive(false);
        Closed?.Invoke(this);
    }

    /// <summary>
    ///     Toggle the user interface on or off.
    /// </summary>
    public virtual void Toggle() {
        if (IsOpen)
            Close();
        else
            Open();
    }
}