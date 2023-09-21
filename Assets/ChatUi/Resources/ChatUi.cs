using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChatUi : MonoBehaviour {
    private ScrollView scroller;
    private TextField txtMessage;
    private Button btnSend;

    public bool printEnteredText = true;
    public event System.Action<string> MessageEntered;

    public Color defaultTextColor = Color.white;
    public int fontSize = 8;


    private void OnEnable() {
        var root = GetComponent<UIDocument>().rootVisualElement;

        scroller = root.Q<ScrollView>();
        txtMessage = root.Q<TextField>();
        btnSend = root.Q<Button>();

        btnSend.clicked +=  BtnSend_Clicked;
        btnSend.SetEnabled(false);
        btnSend.style.fontSize = fontSize;

        txtMessage.RegisterCallback<KeyDownEvent>(TxtMessageKeyDown);
        txtMessage.RegisterValueChangedCallback(TxtMessageChanged);
        txtMessage.style.fontSize = fontSize;

        Label lbl = addEntry("IT4080 Chat UI version 1.0.0", Color.green);
        lbl.style.unityTextAlign = TextAnchor.MiddleCenter;
    }

    
    // ----------------------
    // Private
    // ----------------------
    private void EnterMessage() {
        if (printEnteredText && txtMessage.value != string.Empty) {
            addEntry(txtMessage.value);            
        }

        if (MessageEntered != null) {
            MessageEntered.Invoke(txtMessage.value);
        }

        txtMessage.value = "";
    }



    // ----------------------
    // Events
    // ----------------------
    private void BtnSend_Clicked() {
        EnterMessage();
    }


    private void TxtMessageKeyDown(KeyDownEvent e) {
        if (e.keyCode == KeyCode.Return) {
            EnterMessage();
            txtMessage.Focus();
        }
    }

    private void TxtMessageChanged(ChangeEvent<string> changeEvent) {
        btnSend.SetEnabled(changeEvent.newValue != string.Empty);
    }


    // ----------------------
    // Public
    // ----------------------
    public Label addEntry(string message, Color color) {
        var lbl = new Label(message) {
            style =
            {
                fontSize = fontSize,
                whiteSpace = WhiteSpace.Normal,
                color = color
            }
        };

        scroller.Add(lbl);
        return lbl;
    }


    public Label addEntry(string message) {
        return addEntry(message, defaultTextColor);
    }


    public Label addEntry(string from, string message, Color color)
    {
        return addEntry($"[{from}]:  {message}", color);
    }


    public Label addEntry(string from, string message) {
        return addEntry(from, message, defaultTextColor);
    }

}
