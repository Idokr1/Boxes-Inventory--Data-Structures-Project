using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service
{

    public class Manager
    {
        const int MAX_DIFF = 4;
        private BST<DataX> _mainTree = new BST<DataX>();
        private DoubleLinkedList<BoxByTime> _linkedList = new DoubleLinkedList<BoxByTime>();
        private int _maxItemsPerBox;
        private int _lowAmountInBox;
        private TimeSpan _checkPeriod; //timer check period
        private TimeSpan _expirationDate; //how long does it take until a product with no new sales is deleted
        private Timer _timer;

        public int MaxItemsPerBox { get { return _maxItemsPerBox; } }
        public TimeSpan ExpirationDate { get { return _expirationDate; } }

        public Manager(int maxBoxes, int lowAmount, TimeSpan checkPeriod, TimeSpan expirationDate)
        {
            _mainTree = new BST<DataX>();
            _linkedList = new DoubleLinkedList<BoxByTime>();
            _maxItemsPerBox = maxBoxes;
            _lowAmountInBox = lowAmount;
            _checkPeriod = checkPeriod;
            _expirationDate = expirationDate;
            _timer = new Timer(RemoveUnPopularItems, null, _expirationDate, _checkPeriod);
        }

        /// <summary>
        /// checking if the box exists to insert the new box into the memory correctly
        /// </summary>
        /// <param name="x">the Width & Length of the new box</param>
        /// <param name="y">the Height of the new box</param>
        /// <param name="amount">the amount the user wants to add to the new box</param>
        /// <param name="wasSuccessful">states whether the new box/amount were added successfully</param>
        /// <param name="wereAdded">the amount of units that were actually added</param>
        public void Add(double x, double y, int amount, out bool wasSuccessful, out int wereAdded)
        {
            wasSuccessful = false;
            wereAdded = 0;
            DataX dataX;
            DataY dataY;
            ExistenceStatus status = IsBoxExist(x, y, out dataX, out dataY);
            BoxByTime box = new BoxByTime(x, y);

            switch (status)
            {
                case ExistenceStatus.BothExist: //if both dataX & dataY exist, adding the asked amount to the box (up to a limit that were declared in the configuration)
                    if (_maxItemsPerBox < amount + dataY.Amount)
                    {
                        wereAdded = _maxItemsPerBox - dataY.Amount;
                        dataY.Amount = _maxItemsPerBox;
                    }
                    else
                    {
                        dataY.Amount += amount;
                        wereAdded = amount;
                        wasSuccessful = true;
                    }
                    break;

                case ExistenceStatus.XExist: //if only dataX exists, creating dataY with the asked amount
                    if (amount > _maxItemsPerBox)
                    {
                        wereAdded = _maxItemsPerBox;
                        amount = _maxItemsPerBox;
                    }
                    else
                    {
                        wereAdded = amount;
                        wasSuccessful = true;
                    }
                    dataY = ManageDataY(y, amount, box);
                    dataX.YTree.Add(dataY);
                    break;

                case ExistenceStatus.NotExist: //if both don't exist, creating dataX & dataY with the asked amount
                    if (amount > _maxItemsPerBox)
                    {
                        wereAdded = _maxItemsPerBox;
                        amount = _maxItemsPerBox;
                    }
                    else
                    {
                        wereAdded = amount;
                        wasSuccessful = true;
                    }
                    dataY = ManageDataY(y, amount, box);
                    dataX = new DataX(x);
                    _mainTree.Add(dataX);
                    dataX.YTree.Add(dataY);
                    break;
            }
        }

        /// <summary>
        /// managing all the DataY info by the amount he gets from the user
        /// </summary>
        /// <param name="y">the Height of the box</param>
        /// <param name="amount">the amount the user wants to add to the box</param>
        /// <param name="box">an instance of a box created that added to a linkedlist</param>        
        private DataY ManageDataY(double y, int amount, BoxByTime box)
        {
            DataY dataY;
            if (_maxItemsPerBox < amount) //managing the trees
            {
                dataY = new DataY(y, _maxItemsPerBox);
                _linkedList.AddLast(box); //managing the LinkedList
                dataY.Node = _linkedList.End;
            }
            else
            {
                dataY = new DataY(y, amount);
                _linkedList.AddLast(box); //managing the LinkedList
                dataY.Node = _linkedList.End;
            }
            return dataY;
        }

        /// <summary>
        /// showing info of a box after getting relevant parameters
        /// </summary>
        /// <param name="x">the Width & Length of the box</param>
        /// <param name="y">the Height of the box</param>
        /// <param name="boxExist">checks whether the box exists</param>
        /// <param name="lowAmount">checks whether the box has an equal or lower number of units than stated in the configuration</param>
        /// <param name="amount">providing the amount that was found to the UI</param>
        public void Info(double x, double y, out bool boxExist, out bool lowAmount, out int amount)
        {
            DataX dataX;
            DataY dataY;
            boxExist = false;
            lowAmount = false;
            amount = 0;
            IsBoxExist(x, y, out dataX, out dataY);

            if (dataX != null && dataY != null)
            {
                if (dataY.Amount <= _lowAmountInBox)
                {
                    lowAmount = true;
                }
                boxExist = true;
                amount = dataY.Amount;
                return;
            }
        }

        /// <summary>
        /// getting a demand for amount of units and size of a box, search for it and ask the buyer if he wants to buy it
        /// </summary>
        /// <param name="x">the Width & Length of the box</param>
        /// <param name="y">the Height of the box</param>
        /// <param name="amount">the amount the user wants to add to the box</param>
        /// <param name="matchData">determines whether there is no suitable boxes at all</param>
        /// <param name="sb">create a stringbuilder that holds the info of the found boxes</param>
        /// <param name="boxes">a list of relevant boxes that were found </param>
        public void Buy(double x, double y, int amount, out bool matchData, out StringBuilder sb, out List<Box> boxes)
        {
            bool enoughStock = true;
            double testedX = x;
            double testedY = y;
            int gatheredAmount = 0;
            int newAmount;
            matchData = true;
            sb = new StringBuilder();
            boxes = new List<Box>();
            Box box;
            DataX currentDataX;
            DataY currentDataY;

            while (amount != gatheredAmount)
            {
                _mainTree.SearchEqualOrBigger(new DataX(testedX), out currentDataX); //searching for equal or bigger X (Width / Length)
                if (currentDataX == null) //if the currentDataX is null, checking the current situation and breaks the loop
                {
                    if (gatheredAmount == 0)
                        matchData = false;
                    else if (gatheredAmount != 0)
                        enoughStock = false;
                    break;
                }
                if (currentDataX.X > x + MAX_DIFF) //if the currentDataX.X is higher than the x + MAX_DIFF, checking the current situation and breaks the loop
                {
                    if (gatheredAmount == 0)
                        matchData = false;
                    else if (gatheredAmount != 0)
                        enoughStock = false;
                    break;
                }
                while (amount != gatheredAmount)
                {
                    currentDataX.YTree.SearchEqualOrBigger(new DataY(testedY, 1), out currentDataY); //searching for equal or bigger Y (Height)
                    if (currentDataY == null) //if the currentDataY is null, raising the X by 1 and breaks the loop
                    {
                        testedX = currentDataX.X + 1;
                        testedY = y;
                        break;
                    }
                    if (currentDataY.Y > y + MAX_DIFF) //if the currentDataY.Y is higher than the y + MAX_DIFF, setting testedY back to original Y, raising the X by 1 and breaks the loop
                    {
                        testedX = currentDataX.X + 1;
                        testedY = y;
                        break;
                    }
                    if (amount - gatheredAmount <= currentDataY.Amount) //if the current box has enough stock, add it to the list, reduce its old amount and breaks the loop
                    {
                        int amountBeforePurchase = currentDataY.Amount;
                        newAmount = currentDataY.Amount - (amount - gatheredAmount);
                        gatheredAmount = amount;
                        box = new Box(currentDataX, currentDataY, amountBeforePurchase, newAmount);
                        boxes.Add(box);                        
                        break;
                    }
                    else //if there isn't enough stock, add the available stock to the list, checks whether to keep raising the Y or continuing to the next X
                    {
                        int amountBeforePurchase = currentDataY.Amount;
                        gatheredAmount += currentDataY.Amount;
                        newAmount = 0;
                        box = new Box(currentDataX, currentDataY, amountBeforePurchase, newAmount);
                        boxes.Add(box);                        

                        if (currentDataY.Y == y + MAX_DIFF)
                        {
                            testedX = currentDataX.X + 1;
                            testedY = y;
                            break;
                        }
                        else
                            testedY = currentDataY.Y + 1;
                    }
                }
            }
            if (enoughStock == false)
                sb.Append("We couldn't find enough units with suitable sizes (up to 4 in each of the sides) to fulfill your request, but we can offer you these boxes:\n");
            foreach (var item in boxes)
                sb.Append(item.ToString());
            sb.Append("Do you want to buy it?");
        }

        /// <summary>
        /// deleting boxes/reduce units after a box is bought
        /// </summary>
        /// <param name="boxes">a list of the boxes that were bought </param>
        /// <param name="boxWasDeleted">determines whether an old box was already deleted while the user tarried when he was asked if he wish to buy the box</param>
        /// <param name="sbDeleted">creates a stringbuilder that holds the info of the deleted boxes </param>
        /// <param name="someBoxWasDeleted">checks if at least one box had 0 units and was deleted in order to show a relevant message</param>
        public void ChangeBoughtBoxes(List<Box> boxes, out bool boxWasDeleted, out StringBuilder sbDeleted, out bool someBoxWasDeleted)
        {           
            boxWasDeleted = true;
            someBoxWasDeleted = false;
            sbDeleted = new StringBuilder();
            sbDeleted.Append("The following boxes have now 0 units and therefore they were deleted:\n");
            for (int i = 0; i < boxes.Count; i++)
            {                
                if (boxes[i].AvailableAmount == 0) //if the remaining amount is 0, it removes the box from the linkedlist
                {
                    someBoxWasDeleted = true;
                    sbDeleted.Append($"The dimensions of the deleted box are: Length / Width: {boxes[i].DataX.X}, Height: {boxes[i].DataY.Y}\n");
                    boxes[i].DataX.YTree.Delete(boxes[i].DataY);
                    _linkedList.RemoveByNode(boxes[i].DataY.Node, out boxWasDeleted);
                }
                else //if the remaining amount of the box is higher than 0, it changes the amount, reposite the box to the end of the list and set a new lastBoughtDate
                {
                    boxes[i].DataY.Amount = boxes[i].AvailableAmount;
                    _linkedList.RePositeToEnd(boxes[i].DataY.Node);
                    boxes[i].DataY.Node.Data.lastBoughtDate = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// checking the status of the box based on the provided parameters to determine how to handle the adding process
        /// </summary>
        /// <param name="x">the Width & Length of the box</param>
        /// <param name="y">the Height of the box</param>
        /// <param name="dataX">determine if dataX exists</param>
        /// <param name="dataY">determine if dataY exists</param>
        private ExistenceStatus IsBoxExist(double x, double y, out DataX dataX, out DataY dataY)
        {
            DataX existX;
            DataY existY;
            if (_mainTree.Search(new DataX(x), out existX) && existX.YTree.Search(new DataY(y, default), out existY))
            {
                dataX = existX;
                dataY = existY;
                return ExistenceStatus.BothExist;
            }
            if (_mainTree.Search(new DataX(x), out existX))
            {
                dataX = existX;
                dataY = default;
                return ExistenceStatus.XExist;
            }
            else
            {
                dataY = default;
                dataX = default;
                return ExistenceStatus.NotExist;
            }
        }

        /// <summary>
        /// deleting unpopular boxes when there's no demand for them after a while
        /// </summary>
        /// <param name="state"></param>
        private void RemoveUnPopularItems(object state)
        {
            if (_linkedList.IsEmpty())
            {
                return;
            }
            while ((DateTime.Now - _linkedList.Start.Data.lastBoughtDate) > _expirationDate) //removing from the linkedlist & the BST each box that its expirationDate expired
            {
                BoxByTime boxRemoved;
                _linkedList.RemoveFirst(out boxRemoved);
                DataX currentDataX;
                _mainTree.Search(new DataX(boxRemoved.X), out currentDataX);

                if (currentDataX.YTree.IsDepthIsOne())
                    _mainTree.Delete(currentDataX);
                else
                    currentDataX.YTree.Delete(new DataY(boxRemoved.Y, 1));

                if (_linkedList.IsEmpty())
                    break;
            }
        }

        /// <summary>
        /// showing unpopular boxes that weren't bought for a while, the user decides how long
        /// </summary>
        /// <param name="hours">represents the hours</param>
        /// <param name="minutes">represents the minutes</param>
        /// <param name="seconds">represents the seconds</param>
        public StringBuilder ShowOldBoxes(int hours, int minutes, int seconds)
        {
            StringBuilder oldItems = new StringBuilder();
            TimeSpan checkTime = new TimeSpan(hours, minutes, seconds);
            DoubleLinkedList<BoxByTime> _tempList = new DoubleLinkedList<BoxByTime>(_linkedList); //creating copy ctor so we won't mess the original list

            if (_tempList.IsEmpty())
            {
                oldItems.Append("There are no boxes that haven't been sold for long enough to meet the criteria you entered");
                return oldItems;
            }
            oldItems.Append($"The following boxes haven't been sold for more than {checkTime} time:\n");

            while ((DateTime.Now - _tempList.Start.Data.lastBoughtDate) > checkTime) //checking which boxes meet the criteria that the user entered
            {
                BoxByTime boxRemoved;
                _tempList.RemoveFirst(out boxRemoved);
                oldItems.Append($"{boxRemoved.ToString()}\n");
                if (_tempList.IsEmpty())
                    break;
            }
            return oldItems;
        }
    }
}
