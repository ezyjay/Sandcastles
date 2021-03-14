using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SandDecorationType
{
    //0-100: Side decorations
    STARFISH = 0,
    SHELL = 1,
    PEBBLE = 2,
    SEAWEED = 3,

    //100: Top decorations
    FLAG = 100,
    TWIG = 101,
    BOY = 102,
}

[System.Serializable]
public class Decoration
{
    public SandDecorationType decorationType;
    public GameObject decorationObject;

    public Decoration(SandDecorationType decorationType, GameObject decorationObject) {
        this.decorationType = decorationType;
        this.decorationObject = decorationObject;
    }
}


public class DecorationManager : MonoBehaviour
{
    public Transform decorationsParent;
    public List<Decoration> possibleDecorations = new List<Decoration>();

    private Decoration currentDecoration;
    public Decoration CurrentDecoration { get => currentDecoration; set => currentDecoration = value; }

    public void CreateDecorationObject(SandDecorationType decorationType) {

        GameObject template = possibleDecorations.Find(p => p.decorationType == decorationType).decorationObject;
        if (template != null) {
            GameObject instanciatedDecoration = Instantiate(template, decorationsParent);
            currentDecoration = new Decoration(decorationType, instanciatedDecoration);
        }
    }

    public void ClearAllDecorations() {

        foreach (Transform decorationTransform in decorationsParent) {
            Destroy(decorationTransform.gameObject);
        }
    }
}
