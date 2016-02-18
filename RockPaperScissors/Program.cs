using Leap;
using System;
using System.IO.Ports;
using System.Collections;


/**
 * This is where all "global variables" can be declared. 
 */
public static class MyStaticValues
{
    public static int count = 0; 
}


/**
* This class represents a rock, paper, scissers game. The game is intended to be played between 
* a human and a robotic hand. The human hand is reconized using a Leap Motion camera. The robotic 
* arm and hand are controlled by an arduino Uno and the char that is sent to it throught the 
* moveRobotHand function.
*/
class RockPaperScissors
{

    /**
     * This is the main method wher the game is started from. The game is in an infinate loop
     * allowing the user to play as many rounds as he wants.
     */
    public static void Main()
    {
        RockPaperScissors game = new RockPaperScissors();
        while (true)
        {
            Console.WriteLine("Press enter to start a game... ");
            Console.ReadLine();
            game.playGame();
        }
    }

    /**
     * This method is where the standard rock, paper, scissors logic takes place. Both the 
     * user move and the robot move are compared and a winner is determined. The winner is 
     * then displayed to the screen.
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

        // Displays the winner
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
     * This method gets the users move. A listener is created for the Leap Motion camera. 
     * The users move is obtained from the listener and is returned as the move for this round.
     */
    String getUserMove()
    {
        bool ready = false;
        SampleListener listener = new SampleListener();
        Controller controller = new Controller();
        controller.AddListener(listener);
    
        // Keep this process running until user makes thier move
        while(!ready)
        {
            // Wait until ready to get move
            if (MyStaticValues.count > 3)
            {
                System.Threading.Thread.Sleep(1000);
                ready = true;
                MyStaticValues.count = 0;
            }
        }

       // Console.WriteLine("move = " + listener.move);

        controller.RemoveListener(listener);
        controller.Dispose();

        return listener.move;
    }

    /**
     * This method is where the robot's move is determined. A random number from 1-3 is
     * generated and used in a switch statement to choose either 'rock', 'paper', or 
     * 'scissors'.
     */
    String getRobotMove()
    {
        String move;
        String cMove;

        // Get random number 1-3    
        Random rnd = new Random();
        int random = rnd.Next(1, 4); // creates a number between 1 and 3
        int caseSwitch = random;

        // Set move based off of random number
        switch (caseSwitch)
        {
            case 1:
                move = "rock";
                cMove = "r";
                break;
            case 2:
                move = "paper";
                cMove = "p";
                break;
            case 3:
                move = "scissors";
                cMove = "s";
                break;
            default:
                move = "rock";
                cMove = "r";
                break;
        }

        // Send move to robotic hand
        moveRobotHand(cMove);

        // Return move
        return move;
    }

    /**
     * This method controlls the movement of the robot hand. The move which is
     * to be made by the hand is passed in. 
     */
    void moveRobotHand(String move)
    {
        // Add code to move move robot hand to correct shape
     /*  SerialPort serial = new SerialPort("COM4", 9600);
        serial.Open();
        serial.Write(move);
        serial.Close();*/
    }
}

/**
* This listener captures frames from the Leap Motion camera. Once a listener
* is created it continually gets frames from the camera until the listener is
* removed.
*/
class SampleListener : Listener
{
    public String move = "";

    bool goingDown = false;
    //bool wentDown = false;
    bool goingUp = false;
    

    private Object thisLock = new Object();

    private void SafeWriteLine(String line)
    {
        lock (thisLock)
        {
            Console.WriteLine(line);
        }
    }

    /**
     * This method executes when the listener first connects to the camera.
     */
    public override void OnConnect(Controller controller)
    {
        // SafeWriteLine("Connected");
    }


    /**
     * This method is called each time a frame is captured. 
     */
    public override void OnFrame(Controller controller)
    {
        Frame frame = controller.Frame();
        Hand hand = frame.Hands.Rightmost;

        //The rate of change of the palm position in millimeters/second.
        Vector handSpeed = hand.PalmVelocity;

        // The speed the hand is moving in the y (up and down) direction
        // If y is negative, the hand is moving down
        // If y is positive, the hand is moving up
        float y = handSpeed.y;

        // Console.WriteLine("The hand speed is: " + handSpeed);
        // Console.WriteLine("y: " + y);

        /****************** DETERMINES WHEN TO GET THE USERS MOVE ********************/ 
        // Keeps track of how many times the user moves their hand up and down 
        // The users move should be taken on the fourth downward hand movement (rock, paper, scissors, shoot)
        if (y < -10 || goingDown) // moving down 
        {
            goingDown = true;

            if (y > 10 || goingUp)
            {
                goingUp = true;
                if (y < -10 && goingUp)
                {
                    MyStaticValues.count += 1;
                    // Console.WriteLine("count: " + MyStaticValues.count);
                    goingUp = false;
                    goingDown = false;
                }
            }
        }
        
        /************* DETERMINES THE USERS MOVE BASED OFF OF HAND SHAPE *************/
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
    }
}


/*  public Texture btnTexture;
    void OnGUI()
    {

        if (!btnTexture)
        {
            Debug.LogError("Please assign a texture on the inspector");
            return;
        }
        if (GUI.Button(new Rect(10, 10, 50, 50), btnTexture))
            Debug.Log("Clicked the button with an image");

        if (GUI.Button(new Rect(10, 70, 100, 100), "Start"))
        {
           



            theGame();
        }
    }

    void theGame()
    {
        Debug.Log("Clicked the button with text");

    }*/
