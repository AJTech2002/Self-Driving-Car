using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearRegression;
using System.Linq;

public class GeneticManager : MonoBehaviour
{
    #region Variables
    [Header("References")]
    public SimpleCarController controller;

    [Header("Controllable")]
    public int newPopulation = 60;
    public int population;
    public float mutationChance = 2;

    [Header("Lists")]
    public List<Network> currentGeneration = new List<Network>();
    public List<int> genePool = new List<int>();

    [Header("Privates")]
    public int currentGenerationC = 0;
    public int currentGenome = 0;
    public int mutateTop = 13;

    //[HideInInspector]
    public Network[] currentGenerationFinished;
    #endregion

    //Create a new population of random NetworkStorages
    private void Awake()
    {
        for (int i = 0; i < population+1; i++)
        {
            Network n = new Network();
            n.Initialise(controller.LAYERS,controller.NEURONS);
            currentGeneration.Add(n);
        }
        Next();
    }

    //This controls the next genome or checks when the next generation needs to be created
    public void Next()
    {

       if (currentGenome < population)
        {
            controller.ResetWithNetwork(currentGeneration[currentGenome]);
            currentGenome++;
        }
        else
        {
            population = newPopulation;
            NewGeneration();

            currentGenerationC++;
            currentGenome = 0;
           
            Next();

            
        }
        
    }

    private List<int> tops = new List<int>();
    //This manages creation of new generation
    private void NewGeneration()
    {
        currentGenerationFinished = currentGeneration.ToArray();
        Sort();

        genePool.Clear();
        tops.Clear();

        for (int i = 0; i < mutateTop; i++)
        {
           int genePoolC = Mathf.RoundToInt(currentGenerationFinished[i].fitness/10);

           for (int c = 0; c < genePoolC+1; c++)
           {
                genePool.Add(i);
           }

            tops.Add(i);

        }

        Breed();

    }

    //Breeds the top + creates some new mutations based on rate
    private List<NetworkStorage> storages = new List<NetworkStorage>();
    private void Breed()
    {
        storages.Clear();

        for (int i = 0; i < tops.Count; i++)
        {
            int mutation = Random.Range(0, 101);

            if (mutation > mutationChance)
            {
                Network A = currentGenerationFinished[tops[i]];
                Network B = currentGenerationFinished[genePool[Random.Range(0, genePool.Count)]];

                if (A == B)
                    B = currentGenerationFinished[genePool[Random.Range(0, genePool.Count)]];

                NetworkStorage C = new NetworkStorage(A,i);

                for (int w = 0; w < A.weights.Count; w++)
                {
                    //C.weights[w] = CrossOver(A.weights[w], B.weights[w]);
                    C.weights[w] = CrossOver(A.weights[w], B.weights[w]);
                }

                for (int b = 0; b < A.biases.Count; b++)
                {
                    int AorB = Random.Range(0, 2);
                    if (AorB == 0)
                    {
                        C.biases[b] = A.biases[b];
                    }
                    else
                    {
                        C.biases[b] = B.biases[b];
                    }
                }

                storages.Add(C);
            }
            else
            {
                NetworkStorage C = new NetworkStorage(currentGeneration[i], i);
                for (int w = 0; w < C.weights.Count; w++)
                {
                    C.weights[w] = Mutate(C.weights[w]);
                }

                int b = Random.Range(0, C.biases.Count);
                C.biases[b] = Random.Range(-1f, 1f);
                storages.Add(C);
            }
        }

        for (int i = tops.Count; i < (population); i++)
        {
            int mutation = Random.Range(0, 101);

            if (mutation > mutationChance)
            {
                Network A = currentGenerationFinished[genePool[Random.Range(0, genePool.Count)]];
                Network B = currentGenerationFinished[genePool[Random.Range(0, genePool.Count)]];

                if (A == B)
                    B = currentGenerationFinished[genePool[Random.Range(0, genePool.Count)]];

                NetworkStorage C = new NetworkStorage(A, i);

                for (int w = 0; w < A.weights.Count; w++)
                {
                    //C.weights[w] = CrossOver(A.weights[w], B.weights[w]);
                    C.weights[w] = CrossOver(A.weights[w], B.weights[w]);
                }

                for (int b = 0; b < A.biases.Count; b++)
                {
                    int AorB = Random.Range(0, 2);
                    if (AorB == 0)
                    {
                        C.biases[b] = A.biases[b];
                    }
                    else
                    {
                        C.biases[b] = B.biases[b];
                    }
                }

                storages.Add(C);
            }
            else
            {
                NetworkStorage C = new NetworkStorage(currentGeneration[i], i);
                for (int w = 0; w < C.weights.Count; w++)
                {
                    C.weights[w] = Mutate(currentGeneration[i].weights[w]);
                }

                int b = Random.Range(0, C.biases.Count);
                C.biases[b] = Random.Range(-1f, 1f);
                storages.Add(C);
            }
        }

        for (int i = 0; i < storages.Count; i++)
        {
            currentGeneration[storages[i].index].weights = storages[i].weights;
            currentGeneration[storages[i].index].biases = storages[i].biases;
            currentGeneration[storages[i].index].fitness = 0;
        }

        for (int i = 0; i < currentGeneration.Count; i++)
        {
            currentGeneration[i].fitness = 0;
        }

        #region Shit
        /*for (int i = 0; i < currentGeneration.Count; i++)
        {

            int mutation = Random.Range(0, 101);

            if (mutation > mutationChance)
            {
                Network A = currentGenerationFinished[genePool[Random.Range(0, genePool.Count)]];
                Network B = currentGenerationFinished[genePool[Random.Range(0, genePool.Count)]];

                if (A == B)
                    A = currentGenerationFinished[genePool[Random.Range(0, genePool.Count)]];

                Network C = A;

                for (int w = 0; w < A.weights.Count; w++)
                {
                    C.weights[w] = CrossOver(A.weights[w], B.weights[w]);
                }

                for (int b = 0; b < A.biases.Count; b++)
                {
                    int AorB = Random.Range(0, 2);
                    if (AorB == 0)
                    {
                        C.biases[b] = A.biases[b];
                    }
                    else
                    {
                        C.biases[b] = B.biases[b];
                    }
                }

                currentGeneration[i] = C;
            }
            else
            {
                for (int w = 0; w < currentGeneration[i].weights.Count; w++)
                {
                    currentGeneration[i].weights[w] = Mutate(currentGeneration[i].weights[w]);
                }

                int b = Random.Range(0, currentGeneration[i].biases.Count);
                currentGeneration[i].biases[b] = Random.Range(-1f, 1f);

            }
        }*/
        #endregion

    }

    Matrix<float> Mutate (Matrix<float> A)
    {
        int randomPoints = Random.Range(0, (A.RowCount*A.ColumnCount)/5);

        Matrix<float> C = A;

        for (int i = 0; i < randomPoints+1; i++)
        {
            int randColumn = Random.Range(0, C.ColumnCount);
            int randRow = Random.Range(0, C.RowCount);

            C[randRow, randColumn] = Random.Range(-1f, 1f);

        }
        return C;
    }

    Matrix<float> CrossOver (Matrix<float> A, Matrix<float> B)
    {
        Matrix<float> C = A;
        Matrix<float> D = B;

        int k1 = Mathf.RoundToInt((C.RowCount * C.ColumnCount) * Random.Range(0.00f, 0.400f));
        int k2 = Mathf.RoundToInt((C.RowCount * C.ColumnCount) * Random.Range(0.600f, 1.00f));

        int c = 0;
        for (int x = 0; x < A.RowCount; x++)
        {
            for (int y = 0; y < A.ColumnCount; y++)
            {

                if ( c < k1 || c > k2)
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


        int R = Random.Range(0, 2);
        if (R == 0)
            return C;
        
       return D;
        
    }

    //Cars call this function when they die
    public void Death (float fitness, Network net)
    {
        net.fitness = fitness;
        if (currentGenome < currentGeneration.Count)
        currentGeneration[currentGenome].fitness = fitness;
        Sort();
        Next();
    }
    
    //Sorting algorithm simple
    public void Sort()
    {
         System.Array.Sort(currentGenerationFinished, (left, right) => -left.fitness.CompareTo(right.fitness));
    }

}

[System.Serializable]
public class NetworkStorage
{

    public List<Matrix<float>> weights = new List<Matrix<float>>();
    public List<float> biases = new List<float>();

    public float fitness;
    public int index;

    public NetworkStorage (Network net, int i)
    {
        weights = new List<Matrix<float>>(net.weights);
        biases = new List<float>(net.biases);
        index = i;
    
    }


}