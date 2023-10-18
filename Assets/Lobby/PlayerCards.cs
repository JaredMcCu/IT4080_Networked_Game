
using UnityEngine;
using UnityEngine.UIElements;


public class PlayerCards : VisualElement
{
    private ScrollView list => this.Q<ScrollView>("CardScroll");      
    private VisualTreeAsset playerCardTemplate = Resources.Load<VisualTreeAsset>("PlayerCard");

    public new class UxmlFactory : UxmlFactory<PlayerCards> {}
    public PlayerCards() { }


    public new void Clear() {
        list.Clear();        
    }


    /// <summary>
    /// Creates a PlayerCard with the specified name, adds it to the UI, and
    /// returns it
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public PlayerCard AddCard(string name) {
        var templateInst = playerCardTemplate.Instantiate();
        PlayerCard toReturn = templateInst.Q<PlayerCard>();
        list.Add(toReturn);
        toReturn.playerName = name;
        toReturn.Init();
                
        return toReturn;       
    }

    
}