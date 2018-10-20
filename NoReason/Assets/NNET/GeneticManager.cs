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
    public float maxFitness = 0f;
    public float avgFitness;
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
        for (int i = 0; i < currentGeneration.Count; i++)
        {
            currentGeneration[i].fitness = (currentGeneration[i].fitness / maxFitness);
        }
        currentGenerationFinished = currentGeneration.ToArray();
        Sort();

        genePool.Clear();
        tops.Clear();

      

        for (int i = 0; i < mutateTop; i++)
        {
           
           int genePoolC = Mathf.RoundToInt(currentGenerationFinished[i].fitness*100);

           for (int c = 0; c < genePoolC+1; c++)
           {
                genePool.Add(i);
           }

            tops.Add(i);

        }

        Breed();

    }

    #region Stop
    /*
       for (int i = 0; i<tops.Count; i++)
        {
            if (currentGenerationFinished[tops[i]].fitness > 0.9f)
            {
                print("Save");
    storages.Add(currentGenerationFinished[tops[i]]);
                continue;
            }

int mutation = Random.Range(0, 101);

float mC = mutationChance;
            if (currentGenerationFinished[tops[i]].fitness< 0.4f)
            {
                mC += 2;
                print("MC INCREASE");
            }

            if (mutation > mC)
            {
                Network A = currentGenerationFinished[tops[i]];
Network B = currentGenerationFinished[genePool[Random.Range(0, genePool.Count)]];

                if (A == B)
                    B = currentGenerationFinished[genePool[Random.Range(0, genePool.Count)]];

                Network C = A.InitialiseCopy(controller.LAYERS, controller.NEURONS);

                for (int w = 0; w<A.weights.Count; w++)
                {
                    //C.weights[w] = CrossOver(A.weights[w], B.weights[w]);
                    C.weights[w] = CrossOver(A.weights[w], B.weights[w]);
                }

                for (int b = 0; b<A.biases.Count; b++)
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
                Network C = currentGenerationFinished[i].InitialiseCopy(controller.LAYERS, controller.NEURONS);
                for (int w = 0; w<C.weights.Count; w++)
                {
                    C.weights[w] = Mutate(C.weights[w]);
                }

                int b = Random.Range(0, C.biases.Count);
C.biases[b] = Random.Range(-1f, 1f);

                storages.Add(C);
            }
        }
    */
    #endregion
    //Breeds the top + creates some new mutations based on rate
    private List<Network> storages = new List<Network>();
    private int saveAmount = 1;
    private void Breed()
    {
        storages.Clear();

        int l = 0;

        for (int i = 0; i < (population); i++)
        {
            
            int mutation = Random.Range(0, 101);
            float mC = mutationChance;
            if (currentGenerationFinished[i].fitness < 0.1f)
            {
                mC += 2;
                print("MC INCREASE");
            }

            if (mutation > mC)
            {
                if (currentGenerationFinished[i].fitness > 0.85f)
                {
                    print("Save");
                    storages.Add(currentGenerationFinished[i]);
                    continue;
                }
                else
                {
                    if (currentGenerationFinished[i].fitness < 0.2f)
                    {
                        if (l < saveAmount)
                        {
                            storages.Add(currentGenerationFinished[i]);
                            l++;
                            continue;
                        }
                    }
                    Network A = currentGenerationFinished[genePool[Random.Range(0, genePool.Count)]];
                    Network B = currentGenerationFinished[genePool[Random.Range(0, genePool.Count)]];

                    if (A.weights.Equals(B.weights))
                        B = currentGenerationFinished[genePool[Random.Range(0, genePool.Count)]];

                    Network C = A.InitialiseCopy(controller.LAYERS, controller.NEURONS);

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
            }
            else
            {
                Network C = currentGenerationFinished[i].InitialiseCopy(controller.LAYERS, controller.NEURONS);
                for (int w = 0; w < C.weights.Count; w++)
                {
                    C.weights[w] = Mutate(C.weights[w]);
                }

                int b = Random.Range(0, C.biases.Count);
                C.biases[b] = Mathf.Clamp(C.biases[b]+Random.Range(-0.1f, 0.1f),-1f,1f);
                storages.Add(C);
            }
        }

        for (int i = 0; i < storages.Count; i++)
        {
            currentGeneration[i].weights = storages[i].weights;
            currentGeneration[i].biases = storages[i].biases;
            currentGeneration[i].fitness = 0;
        }

        for (int i = 0; i < currentGeneration.Count; i++)
        {
            currentGeneration[i].fitness = 0;
        }
        maxFitness = 0f;
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

            C[randRow, randColumn] = Mathf.Clamp(C[randRow, randColumn]+Random.Range(-0.1f, 0.11f),-1f,1f);

        }
        return C;
    }

    Matrix<float> CrossOver (Matrix<float> A, Matrix<float> B)
    {
        Matrix<float> C = A;
        Matrix<float> D = B;

        int k1 = Mathf.RoundToInt((C.RowCount * C.ColumnCount) * Random.Range(0.00f, 0.300f));
        int k2 = Mathf.RoundToInt((C.RowCount * C.ColumnCount) * Random.Range(0.700f, 1.00f));

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
        avgFitness = (avgFitness + fitness) / 2;

        maxFitness += fitness;

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