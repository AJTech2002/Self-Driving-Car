using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class GeneticManagerTwo : MonoBehaviour
{
    #region Public Variables

    [Header("Saves")]
    [Multiline]
    public string JSON;

    [Header("References")]
    public SimpleCarController carController;
    public GraphHelp grapher;

    [Header("Public Controls")]
    public int initialPopulation;
    [Range(0.0f, 1.0f)]
    public float mutationRate = 0f;

    [Header("Crossover Controls")]
    public int bestAgentSelection = 6;
    public int worstAgentSelection = 3;
    public int numberToCrossover = 4;

    //Create the initialPopulation
    private Network[] population;

    //Copied into this for the top networks
    private Network[] newPopulation;

    [Header("Public View")]
    public int currentGeneration = 0;
    public int currentGenome = 0;
    //Per Generation
    public int mutationCount = 0;

    #endregion

    private void Start()
    {
        grapher.AddGraph("Avg", Color.red);
        grapher.Plot(0, 0, "Avg");
        InstaPopulation();
    }

    #region Population management

    private void InstaPopulation()
    {
        population = new Network[initialPopulation];

        FillPopulationByRandomValues(population, 0);
        ResetToCurrentGenome();
    }

    private void FillPopulationByRandomValues (Network[] newPopulation, int startIndex)
    {
        while (startIndex < initialPopulation)
        {
            newPopulation[startIndex] = new Network();
    
            newPopulation[startIndex].Initialise(carController.LAYERS, carController.NEURONS);
            
            startIndex++;
        }
    }

    public string SerializeNetwork (Network net) {

        return "";
    }
    
    public void Death(float fitness, Network network)
    {
        if (currentGenome < population.Length-1)
        {
            
            population[currentGenome].fitness = fitness;
            if (currentGenome < numberToCrossover*2)
             avgFitness += fitness;
            currentGenome++;
            ResetToCurrentGenome();    
        }
        else
        {
            RePop();
            avgFitness = 0f;
        }
    }

    private int newPopulationIndex = 0;
    private float avgFitness = 0f;

    private void RePop()
    {
        genePool.Clear();
        avgFitness /=  numberToCrossover*2;
        grapher.Plot(currentGeneration+1, avgFitness, "Avg");

        currentGeneration++;

        newPopulationIndex = 0;

        SortPopulation();
        Network[] newPopulation = PickBestPopulation();

        CrossOver(newPopulation);
        Mutate(newPopulation);

        //Wtf is this here for
        FillPopulationByRandomValues(newPopulation, newPopulationIndex);

        population = newPopulation;

        currentGenome = 0;
        ResetToCurrentGenome();

    }

    private void ResetToCurrentGenome()
    {
        carController.ResetWithNetwork(population[currentGenome]);
    }

    #endregion

    #region RouletteSelection

    // Returns the selected index based on the weights(probabilities)
    int rouletteSelect(double[] weight)
    {
        // calculate the total weight
        double weight_sum = 0;
        for (int i = 0; i < weight.Length; i++)
        {
            weight_sum += weight[i];
        }
        // get a random value
        double value = randUniformPositive() * weight_sum;
        // locate the random value based on the weights
        for (int i = 0; i < weight.Length; i++)
        {
            value -= weight[i];
            if (value < 0) return i;
        }
        // when rounding errors occur, we return the last item's index 
        return weight.Length - 1;
    }

    // Returns a uniformly distributed double value between 0.0 and 1.0
    double randUniformPositive()
    {
        // easiest implementation
        return Random.Range(0.0f, 1.0f);
    }

    #endregion

    #region Selection Algorithm v2
    //Sort the initial population[]
    private void SortPopulation()
    {
        for (int i = 0; i < population.Length; i++)
        {
            for (int j = i; j < population.Length; j++)
            {
                if (population[i].fitness < population[j].fitness)
                {
                    Network Temp = population[i];
                    population[i] = population[j];
                    population[j] = Temp;
                }
            }
        }
    }

    private List<int> genePool = new List<int>();
    private Network[] PickBestPopulation()
    {
        Network[] newPopulation = new Network[initialPopulation];

        for (int i = 0; i < bestAgentSelection; i++)
        {

            newPopulation[newPopulationIndex] = population[i].InitialiseCopy(carController.LAYERS, carController.NEURONS);

            int f = Mathf.RoundToInt(population[i].fitness*10);
            for (int c = 0; c < f; c++)
            {
                genePool.Add(i);
            }

           /* for (int x = 0; x < population[i].weights.Count; x++)
            {
                Matrix<float> setCopy = Matrix<float>.Build.Dense(population[i].weights[x].RowCount, population[i].weights[x].ColumnCount);
                population[i].weights[x].CopyTo(setCopy);
                newPopulation[newPopulationIndex].weights.Add(setCopy);
            } */

            //newPopulation[newPopulationIndex].biases = new List<float>(population[i].biases);

            newPopulation[newPopulationIndex].fitness = 0;
            newPopulationIndex++;
        }

        for (int i = 0; i < worstAgentSelection; i++) {
            int last = population.Length-1;
            last -= i;
            int f = Mathf.RoundToInt(population[last].fitness*10);
            for (int c = 0; c < f; c++)
            {
                genePool.Add(last);
            }
        }

        return newPopulation;
    }

    #endregion

    #region Crossover and Mutation
    (Matrix<float>,Matrix<float>) CrossOver(Matrix<float> A, Matrix<float> B)
    {
        newPopulationIndex = 0;
        Matrix<float> C = A.Clone();
        Matrix<float> D = B.Clone();

        int k1 = Mathf.RoundToInt((C.RowCount * C.ColumnCount) * Random.Range(0f, 1f));
        int k2 = Mathf.RoundToInt((C.RowCount * C.ColumnCount) * Random.Range(0f, 1f));

        int c = 0;
        for (int x = 0; x < A.RowCount; x++)
        {
            for (int y = 0; y < A.ColumnCount; y++)
            {

                if (c < k1 || c > k2)
                {
                    C[x, y] = A[x, y];
                    D[x, y] = B[x, y];
                }
                else
                {
                    C[x, y] = B[x, y];
                    D[x, y] = A[x, y];
                }

                c++;
            }
        }


        return (C, D);

    }

    private void CrossOver(Network[] newPopulation)
    {
        for (int i = 0; i < numberToCrossover; i += 2)
        {
            int AIndex = i;
            int BIndex = i+1;
            if (genePool.Count >= 1) {
                for (int l = 0; l < 100; l++) {
                    AIndex = genePool[Random.Range(0, genePool.Count)];
                    BIndex = genePool[Random.Range(0, genePool.Count)];

                    if (AIndex != BIndex)
                    {
                        break;
                    }

                }
            }

            Network Child1 = new Network();
            Network Child2 = new Network();
            Child1.Initialise(carController.LAYERS, carController.NEURONS);
            Child2.Initialise(carController.LAYERS, carController.NEURONS);

            Child1.fitness = 0;
            Child2.fitness = 0;

           for (int w = 0; w < Child1.weights.Count; w++)
            {
                if (Random.Range(0.0f, 1.0f) < 0.5f)
                {//(Matrix<float> A, Matrix<float> B) = CrossOver(population[AIndex].weights[w], population[BIndex].weights[w]);
                    Child1.weights[w]=(population[AIndex].weights[w]);
                    Child2.weights[w]=(population[BIndex].weights[w]);
                }
                else {
                    Child1.weights[w]=(population[BIndex].weights[w]);
                    Child2.weights[w]=(population[AIndex].weights[w]);
                }
            }

           for (int b = 0; b < Child1.biases.Count; b++)
            {
                if (Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    Child1.biases[b]=(population[AIndex].biases[b]);
                    Child2.biases[b]=(population[BIndex].biases[b]);
                }
                else
                {
                    Child1.biases[b] = (population[BIndex].biases[b]);
                    Child2.biases[b] = (population[AIndex].biases[b]);
                }
            }


            newPopulation[newPopulationIndex] = Child1;
            newPopulationIndex++;

            newPopulation[newPopulationIndex] = Child2;
            newPopulationIndex++;
        }
    }

    private void Mutate(Network[] newPopulation)
    {
        for (int i = 0; i < newPopulationIndex; i++)
        {
            for (int c = 0; c < newPopulation[i].weights.Count; c++)
            {
                if (Random.Range(0.0f, 1.0f) < mutationRate)
                {
                    newPopulation[i].weights[c] = Mutate(newPopulation[i].weights[c]);
                }
            }

            if (Random.Range(0.0f, 1.0f) < mutationRate)
            {
                int b = Random.Range(0, newPopulation[i].biases.Count);
                newPopulation[i].biases[b] = Mathf.Clamp(newPopulation[i].biases[b] + Random.Range(-1f, 1f), -1f, 1f);

            }
        }
    }

    Matrix<float> Mutate(Matrix<float> A)
    {
        int randomPoints = Random.Range(0, (A.RowCount * A.ColumnCount) / 7);

        Matrix<float> C = A;

        for (int i = 0; i < randomPoints + 1; i++)
        {
            int randColumn = Random.Range(0, C.ColumnCount);
            int randRow = Random.Range(0, C.RowCount);

            //Nudge Value in a random place up or down a bit
            C[randRow, randColumn] = Mathf.Clamp(C[randRow, randColumn] + Random.Range(-1f, 1f), -1f, 1f);

        }
        return C;
    }

    #endregion

}


#region External Classes
[System.Serializable]
public class NetworkStorage
{

    public List<Matrix<float>> weights = new List<Matrix<float>>();
    public List<float> biases = new List<float>();

    public float fitness;
    public int index;

    public NetworkStorage(Network net, int i)
    {

        for (int x = 0; x < net.weights.Count; x++)
        {
            weights.Add(net.weights[x].Clone());
        }

        biases = new List<float>(net.biases);
        index = i;

    }


}
#endregion