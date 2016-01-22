using Leap;
using System;

/**
*
*/
class RockPaperScissors
{
    /**
     * 
     */
    public static void Main()
    {
        RockPaperScissors game = new RockPaperScissors();
        while (true)
        {
            Console.WriteLine("Press any key to start a game... ");
            Console.ReadLine();
            game.playGame();
        }
    }

    /**
     *
     */
    void playGame()
    {
        String userMove = getUserMove();
        String robotMove = getRobotMove();
        String winner = "";

        // If there is a tie
        if (userMove.Equals(robotMove))
        {
            winner = "tie";
        }
        // If robot chooses 'rock'
        else if (robotMove.Equals("rock"))
        {
            if (userMove.Equals("paper"))
            {
                winner = "human";
            }
            else if (userMove.Equals("scissors"))
            {
                winner = "robot";
            }
        }
        // If robot chooses 'paper'
        else if (robotMove.Equals("paper"))
        {
            if (userMove.Equals("scissors"))
            {
                winner = "human";
            }
            else if (userMove.Equals("rock"))
            {
                winner = "robot";
            }
        }
        // If robot chooses 'scissors'
        else if (robotMove.Equals("scissors"))
        {
            if (userMove.Equals("rock"))
            {
                winner = "human";
            }
            else if (userMove.Equals("paper"))
            {
                winner = "robot";
            }
        }

        Console.WriteLine("The human did a " + userMove + "\nThe robot did a " + robotMove + "\n\n");
        if (winner.Equals("tie"))
        {
            Console.WriteLine("It is a tie!");
        }
        else
        {
           Console.WriteLine("The " + winner + " wins!!!!\n");
        }
    }

    /**
     *
     */
    String getUserMove()
    {
        SampleListener listener = new SampleListener();
        Controller controller = new Controller();
        controller.AddListener(listener);

        //  ******************************************************** THIS NEEDS DONE *************************************      
        // Keep this process running until enter is pressed
        // we want to keep this to keep running until the move is made
        Console.WriteLine("Press enter to get your move...");
        Console.ReadLine();
        //  **************************************************************************************************************      


       // Console.WriteLine("move = " + listener.move);

        controller.RemoveListener(listener);
        controller.Dispose();

        return listener.move;
    }

    /**
     *
     */
    String getRobotMove()
    {
        String move;

        // Get random number 1-3    
        Random rnd = new Random();
        int random = rnd.Next(1, 4); // creates a number between 1 and 3
        int caseSwitch = random;

        // Set move based off of random number
        switch (caseSwitch)
        {
            case 1:
                move = "rock";
                break;
            case 2:
                move = "paper";
                break;
            case 3:
                move = "scissors";
                break;
            default:
                move = "rock";
                break;

        }
                // send move to robotic hand (will probably need a nother function for this)


                // Return move
               return move;
    }
}

/**
*
*/
class SampleListener : Listener
{
    // used to slow down frames
    int count = 0;
    public String move = "";

    private Object thisLock = new Object();

    private void SafeWriteLine(String line)
    {
        lock (thisLock)
        {
            Console.WriteLine(line);
        }
    }

    public override void OnConnect(Controller controller)
    {
        // SafeWriteLine("Connected");
    }

    public override void OnFrame(Controller controller)
    {
        // Slows down the frames
        count++;


        //  if (count > 50)
        {
            Frame frame = controller.Frame();
            Hand hand = frame.Hands.Rightmost;


            //The rate of change of the palm position in millimeters/second.
            Vector handSpeed = hand.PalmVelocity;
            // Console.WriteLine("The hand speed is: " + handSpeed);

            //if (handSpeed < )
            //{

            // Used to check for "rock"
            float strength = hand.GrabStrength;

            // USed to check for "scissors"
            FingerList fingers = hand.Fingers.Extended();

            // Used to check for "paper"
            float pitch = hand.Direction.Pitch;
            float yaw = hand.Direction.Yaw;
            float roll = hand.PalmNormal.Roll;

            // Check for "rock"
            if (strength > .9) //  [0..1]. 0 open 1 fist
            {
                move = "rock";
            }
            // Check for scissors
            else if (fingers.Count > 0 && fingers.Count < 4)
            {
                move = "scissors";
            }
            // Check for "paper"
            else //if (pitch < .5 && yaw < .5 && roll < .5 /*&& !move.Equals("rock")*/)
            {
                move = "paper";
            }
           /* else
            {
                move = "invalid";
            }*/


               /* Console.WriteLine("Strength is: " + strength);
                Console.WriteLine("pitch  = " + pitch);
                Console.WriteLine("yaw  = " + yaw);
                Console.WriteLine("roll  = " + roll);
                Console.WriteLine("fingers  = " + fingers);*/

            //  Console.WriteLine("\nmove = " + move + "\n");

            //}
            count = 0;
        }
    }
}

