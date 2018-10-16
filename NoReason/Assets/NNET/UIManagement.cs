using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManagement : MonoBehaviour
{
    #region Public Variables
    public bool work;

    [Header("Facts")]
    public Network currentNetwork;

    [Header("Network Elements")]
    public List<Image> aNeuron;
    public List<Image> bNeuron;
    public List<Image> cNeuron;
    public List<Image> HiddenANeuron;
    public List<Image> HiddenBNeuron;
    public List<Image> HiddenCNeuron;

    public List<Image> inputs = new List<Image>();
    public List<Image> hidden = new List<Image>();
    public List<Image> outputs = new List<Image>();

    public Text output1Text;
    public Text output2Text;

    public Text hidden1Text;
    public Text hidden2Text;
    public Text hidden3text;

    public Text inp1T;
    public Text inp2T;
    public Text inp3T;
    #endregion

    /*
    private void Update()
    {
        if (currentNetwork == null)
            currentNetwork = GameObject.FindObjectOfType<SimpleCarController>().network;

        if (work && currentNetwork != null)
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                Color c = colorOutInput(currentNetwork.inputLayer[0, i]);
                inputs[i].color = c;
            }

            for (int i = 0; i < hidden.Count; i++)
            {
                Color c = colorOutCustom(currentNetwork.hiddenLayer[0,i], hiddenL, hiddenH);
                hidden[i].color = c;
            }

            for (int i = 0; i < outputs.Count; i++)
            {
                Color c = colorOutCustom(currentNetwork.outputLayer[0,i], outputL, outputH);
                outputs[i].color = c;
            }

            output1Text.text = currentNetwork.outputLayer[0, 0].ToString();
            output2Text.text = currentNetwork.outputLayer[0, 1].ToString();

            hidden1Text.text = currentNetwork.hiddenLayer[0, 0].ToString();
            hidden2Text.text = currentNetwork.hiddenLayer[0, 1].ToString() ;
            hidden3text.text = currentNetwork.hiddenLayer[0, 2].ToString() ;

            inp1T.text = currentNetwork.inputLayer[0, 0].ToString();
            inp2T.text = currentNetwork.inputLayer[0, 1].ToString();
            inp3T.text = currentNetwork.inputLayer[0, 2].ToString();

            for (int i = 0; i < aNeuron.Count; i++)
            {
                Color c = colorOut(currentNetwork.inputToHidden[i, 0]*currentNetwork.inputLayer[0,0]);
                aNeuron[i].color = c;
            }

            for (int i = 0; i < bNeuron.Count; i++)
            {
                Color c = colorOut(currentNetwork.inputToHidden[i, 1] * currentNetwork.inputLayer[0, 1]);
                bNeuron[i].color = c;
            }

            for (int i = 0; i < cNeuron.Count; i++)
            {
                Color c = colorOut(currentNetwork.inputToHidden[i, 2] * currentNetwork.inputLayer[0, 2]);
                cNeuron[i].color = c;
            }

            for (int i = 0; i < HiddenANeuron.Count; i++)
            {
                Color c = colorOutB(currentNetwork.hiddenToOutput[0, i] * currentNetwork.hiddenLayer[0, 0]);
                HiddenANeuron[i].color = c;
            }

            for (int i = 0; i < HiddenBNeuron.Count; i++)
            {
                Color c = colorOutB(currentNetwork.hiddenToOutput[1, i] * currentNetwork.hiddenLayer[0, 1]);
                HiddenBNeuron[i].color = c;
            }

            for (int i = 0; i < HiddenCNeuron.Count; i++)
            {
                Color c = colorOutB(currentNetwork.hiddenToOutput[2, i] * currentNetwork.hiddenLayer[0, 2]);
                HiddenCNeuron[i].color = c;
            }


        }
    }
    */
    #region Color Management
    float lowest = -1;
    float highest = 1;
    private Color colorOut (float inColor)
    {
        if (inColor < lowest)
            lowest = inColor;
        if (inColor > highest)
            highest = inColor;

        float percent = (inColor - lowest) / (highest - lowest);

        Color lerp = Color.Lerp(Color.green, Color.red, percent);
        return lerp;
    }

    float lowestB = -1;
    float highestB = 1;
    private Color colorOutB(float inColor)
    {
        if (inColor < lowestB)
            lowestB = inColor;
        if (inColor > highestB)
            highestB = inColor;

        float percent = (inColor - lowestB) / (highestB - lowestB);

        Color lerp = Color.Lerp(Color.green, Color.red, percent);
        return lerp;
    }

    float inputsH = 0;
    float inputsL = 100;

    float hiddenH = 1;
    float hiddenL = -1;

    float outputH = 1;
    float outputL = -1;

    private Color colorOutCustom(float inColor, float lowestB, float highestB)
    {
        if (inColor < lowestB)
            lowestB = inColor;
        if (inColor > highestB)
            highestB = inColor;

        float percent = (inColor - lowestB) / (highestB - lowestB);

        Color lerp = Color.Lerp(Color.green, Color.red, percent);
        return lerp;
    }

    private Color colorOutInput (float inColor)
    {
        if (inColor < inputsL)
            inputsL = inColor;
        if (inColor > inputsH)
            inputsH = inColor;

        float percent = (inColor - inputsL) / (inputsH - inputsL);

        Color lerp = Color.Lerp(Color.green, Color.red, percent);
        return lerp;
    }
    #endregion
}
