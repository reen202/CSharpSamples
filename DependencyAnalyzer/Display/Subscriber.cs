using System.Collections;
using System;
using System.IO;

namespace GUI
{

    public class GUIEventListener
    {

        private ArrayList<Listener> listeners
           = new ArrayList<Listener>();
        private int state;

        public int getState()
        {
            return state;
        }

        public void setState(int state)
        {
            this.state = state;
            notifyAllListeners();
        }

        public void attach(Listener listener)
        {
            listeners.add(listener);
        }

        public void notifyAllListeners()
        {
            foreach (Listener listener in listeners)
            {
                listener.update();
            }
        }
    }

    public abstract class Listener
    {
        protected Subject subject;
        public abstract void update();
    }

    public class CheckInListener : Listener
    {

        public CheckInListener(Subject subject)
        {
            this.subject = subject;
            this.subject.attach(this);
        }


        public void update()
        {
            Console.WriteLine("Client has clicked on check in button");
        }
    }

    public class CheckOutListener : Listener
    {

        public CheckOutListener(Subject subject)
        {
            this.subject = subject;
            this.subject.attach(this);
        }

        public void update()
        {
            Console.WriteLine("Client has clicked on check out button");
        }
    }

    public class ReportListener : Listener
    {

        public ReportListener(Subject subject)
        {
            this.subject = subject;
            this.subject.attach(this);
        }


        public void update()
        {
            Console.WriteLine("Client has requested reports");
        }
    }
    public class Subject
    {
        public void attach(Object listener);
        public void notifyAllListeners()
        {
            /*Operations to notify all listeners*/

        }
    }
    public class ObserverPatternDemo
    {
        public static void main(String[] args)
        {
            Subject subject = new Subject();
            //registering all listeners
            new CheckInListener(subject);
            new CheckOutListener(subject);
            new ReportListener(subject);
            subject.notifyAllListeners();
        }
    }
}