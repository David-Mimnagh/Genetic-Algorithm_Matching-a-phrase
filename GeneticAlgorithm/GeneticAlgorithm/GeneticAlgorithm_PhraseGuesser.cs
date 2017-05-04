using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class Element
    {
        public string GuessedPhrase { get; set; }
        public double Fitness { get; set; }
        public double Probability { get; set; }

    }
    class GeneticAlgorithm_PhraseGuesser
    {
        public static string PhraseToGuess { get; set; }
        public static Random rng = new Random();
        public static double MutationRate { get; set; }
        public static List<Element> MatingPool { get; set; }

        static char newRandomChar()
        {
            string possChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ .";
            List<char> characters = possChars.ToCharArray().ToList();

            return characters[rng.Next(characters.Count)];
        }

        static void NaturalSelection(List<Element> population)
        {
            for ( var i = 0; i < population.Count; i++ )
            {
                int number = (int)Math.Floor(population[i].Fitness * 100);  // Arbitrary multiplier, we can also use monte carlo method
                for ( var j = 0; j < number; j++ )
                {              // and pick two random numbers
                    MatingPool.Add(population[i]);
                }
            }
        }

        static void InitializePopulation(List<Element> population, int size)
        {
            foreach ( var p in population )
            {
                while ( p.GuessedPhrase.Length != PhraseToGuess.Length )
                {
                    p.GuessedPhrase += newRandomChar();
                }
            }


        }

        static void CalulateFitness(Element e)
        {
            int score = 0;
            for ( int i = 0; i < e.GuessedPhrase.Length; i++ )
            {
                if ( e.GuessedPhrase[i] == PhraseToGuess[i] )
                {
                    score++;
                }
            }
            e.Fitness = (double)( (double)score / (double)PhraseToGuess.Length );
        }

        static Element CrossOver(Element parentA, Element parentB)
        {
            var midPoint = Math.Floor((double)rng.Next(parentA.GuessedPhrase.Length));
            Element newChild = new Element();
            string newPhrase = "";
            for ( int i = 0; i < parentA.GuessedPhrase.Length; i++ )
            {
                if ( i > (int)midPoint )
                    newPhrase += parentA.GuessedPhrase[i];
                else
                    newPhrase += parentB.GuessedPhrase[i];
            }
            newChild.GuessedPhrase = newPhrase;
            newChild.Fitness = ( parentA.Fitness + parentB.Fitness ) / 2;
            return newChild;
        }


        static Element Mutation(Element e)
        {
            string phraseAfterMutation = "";
            for ( int i = 0; i < e.GuessedPhrase.Length; i++ )
            {
                if ( rng.NextDouble() <= MutationRate )
                {
                    phraseAfterMutation += newRandomChar();
                }
                else
                {
                    phraseAfterMutation += e.GuessedPhrase[i];
                }
            }
            e.GuessedPhrase = phraseAfterMutation;
            return e;
        }

        static List<Element> NewGeneration(List<Element> currentPop)
        {
            for ( var i = 0; i < currentPop.Count; i++ )
            {
                int a = (int)Math.Floor((double)rng.Next(MatingPool.Count));
                int b = (int)Math.Floor((double)rng.Next(MatingPool.Count));
                var partnerA = MatingPool[a];
                var partnerB = MatingPool[b];
                var child = CrossOver(partnerA, partnerB);
                child = Mutation(child);
                currentPop[i] = child;
            }
            return currentPop;
        }

        static void Main(string[] args)
        {
            PhraseToGuess = "David Richmond Mimnagh";
            int popSize = 570;//((PhraseToGuess.Length * 3) * 100);
            MutationRate = 0.01;

            List<Element> population = new List<Element>();
            List<Element> newPopulation = new List<Element>();
            MatingPool = new List<Element>();

            for ( int i = 0; i < popSize; i++ )
            {
                population.Add(new Element { GuessedPhrase = "", Fitness = 0.0f, Probability = 0.0f });
            }

            List<string> guessedPhrases = new List<string>();



            Console.WriteLine("Here is the phrase that the computer is trying to guess: '" + PhraseToGuess + "'");

            // 1 - Initialize - Create a population of N elements, each with randomly generated DNA
            InitializePopulation(population, popSize);


            // While the list of guessed words !contain the phrase to guess, keep going
            while ( !guessedPhrases.Contains(PhraseToGuess) )
            {

                //2- Selection - Evaluate the fitness of each element of the population and build a mating pool
                foreach ( var p in population )
                {
                    CalulateFitness(p); // Get the fitness of the current population
                }

                //Reproduction - Repeat N times
                MatingPool = new List<Element>(); // Build up the mating pool
                NaturalSelection(population); // Retrieve the best from the current population

                newPopulation = new List<Element>();
                newPopulation = NewGeneration(population); // Make a new generation from the mating pool

                //Add all the new guesses from this generation
                foreach ( var nP in newPopulation )
                {
                    if ( !guessedPhrases.Contains(nP.GuessedPhrase) )
                    {
                        guessedPhrases.Add(nP.GuessedPhrase);
                    }
                }

                population = newPopulation; // make the new population the now current population for the next run

                population = population.OrderByDescending(x => x.Fitness).ToList(); // Not necessary, just needed to show the best guess

                //Show the best phrase from this generation
                Console.WriteLine("Best Guess from generation #" + ( guessedPhrases.Count / popSize ).ToString() + ": " + population.First().GuessedPhrase);
                Console.WriteLine("Fitness: " + population.First().Fitness.ToString());
            }
            population = population.OrderByDescending(x => x.Fitness).ToList(); // Not necessary, just needed to show the best guess
            //Some information for the user
            Console.WriteLine("\n\n----Managed to get the phrase within generation # " + ( (guessedPhrases.Count / popSize) +1).ToString() + ".");
            Console.WriteLine("Phrase given: '" + PhraseToGuess + "'");
            Console.WriteLine("Total Guesses: " + ( guessedPhrases.Count ).ToString());
            Console.WriteLine("Population Size: " + popSize.ToString());
            Console.WriteLine("Mutation Rate: " + MutationRate.ToString());
            Console.WriteLine("Best Fitness: " + population.First().Fitness.ToString());
            Console.ReadLine();
        }
    }
}
