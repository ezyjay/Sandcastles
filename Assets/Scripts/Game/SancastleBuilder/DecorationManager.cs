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

    private List<GameObject> allBuiltDecorations = new List<GameObject>();
    private List<GameObject> undoneDecorations = new List<GameObject>();

    public void CreateDecorationObject(SandDecorationType decorationType, bool destroyPreviousObject = false, bool commitCurrentObject = false) {

        // Destroy previous decoration object if there was one
        if (destroyPreviousObject)
            DestroyCurrentDecoration();

        //Add object to list of built decorations
        if (commitCurrentObject && currentDecoration != null && currentDecoration.decorationObject != null)
            allBuiltDecorations.Add(currentDecoration.decorationObject);

        // Create new object
        GameObject template = possibleDecorations.Find(p => p.decorationType == decorationType).decorationObject;
        if (template != null) {
            GameObject instanciatedDecoration = Instantiate(template, decorationsParent);
            currentDecoration = new Decoration(decorationType, instanciatedDecoration);
        }
    }

    public void DestroyCurrentDecoration() {
        if (currentDecoration != null && currentDecoration.decorationObject != null) {
            Destroy(currentDecoration.decorationObject);
            currentDecoration.decorationObject = null;
        }
    }


    public void ClearAllDecorations() {

        foreach (Transform decorationTransform in decorationsParent) {
            Destroy(decorationTransform.gameObject);
        }
    }
}
