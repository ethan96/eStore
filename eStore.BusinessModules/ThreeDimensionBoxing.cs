using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;

namespace eStore.BusinessModules
{
    public class ThreeDimensionBoxing
    {
        private bool _addExtraLength = false;

        private bool _usePrebuiltContainer = false;

        public bool UsePrebuiltContainer { get { return _usePrebuiltContainer; } set { _usePrebuiltContainer = value; } }

        public bool AddExtraLengthIsRequired { get { return _addExtraLength; } set { _addExtraLength = value; } }

        private List<Container> _availableContainers;

        private List<ProductShippingDimension> _waitingBoxes;

        public List<USPSPackage> getAvailableUSPSPackages()
        {
            var packages = new List<USPSPackage>();
            USPSPackageHelper helper = new USPSPackageHelper();
            packages = helper.getUSPSPackages();
            return packages;
        }

        public List<USPSPackage> getAvailablePackages()
        {
            var packages = getAvailableUSPSPackages();
            return packages;
        }
        
        public List<Container> GetUSPSContainers()
        {
            var uspspackages = (from u in getAvailablePackages()
                                select new Container
                                {
                                    ID = u.ID,
                                    PartNo = u.Description,
                                    Width = u.Length,
                                    Depth = u.Width,
                                    Height = u.Depth,
                                    Arrangements = new List<Arrangement>()
                                }).ToList();
            return uspspackages;
        }

        public List<Container> GetAvailableContainers(string storeName, bool upsOnly = false)
        {
            if (_availableContainers == null)
            {
                /*
                BoxHelper helper = new BoxHelper();
                _availableContainers = (from b in helper.getBoxes(storeName)
                                        select new Container
                                        {
                                            ID = b.BoxID,
                                            PartNo = string.Format("{0}", b.BoxID),
                                            Width = b.WidthINCH,
                                            Depth = b.LengthINCH,
                                            Height = b.HighINCH,
                                            Arrangements = new List<Arrangement>()
                                        }).ToList();
                */
                // append uspspackages into available containers
                if (!upsOnly)
                {
                    var uspspackages = (from u in getAvailablePackages()
                                        select new Container
                                        {
                                            ID = u.ID,
                                            PartNo = u.Description,
                                            Width = u.Length,
                                            Depth = u.Width,
                                            Height = u.Depth,
                                            ContainerName = u.Container,
                                            Arrangements = new List<Arrangement>()
                                        }).ToList();
                    _availableContainers = new List<Container>();
                    _availableContainers.AddRange(uspspackages);
                }
                return _availableContainers;
            }
            else
                return _availableContainers;
        }


        public List<ProductShippingDimension> GetWaitingBoxes
        {
            get
            {
                if (_waitingBoxes == null)
                    _waitingBoxes = new List<ProductShippingDimension>();
                return _waitingBoxes;
            }
            set
            {
                _waitingBoxes = value;
            }
        }

        public List<Container> Boxing(string storeName, List<ProductShippingDimension> boxes)
        {
            // extra spaces will be required whenever AddExtraLengthIsRequired is true and box number greater than 1
            if (boxes.Count == 0) return null;
            var containerList = new List<Container>();
            var sortedBoxes = boxes.OrderBy(x => x.Height).OrderByDescending(x => x.Surface).ToList();
            // find the most suitable container with smallest height
            var container = FindSuitableContainer(storeName, sortedBoxes);
            /// No suitable container found, treate first box as a container
            if (container == null)
            {
                container = new Container
                {
                    ID = sortedBoxes[0].ID,
                    PartNo = sortedBoxes[0].PartNo,
                    Width = sortedBoxes[0].Width,
                    Height = sortedBoxes[0].Height,
                    Depth = sortedBoxes[0].Depth,
                    Arrangements = new List<Arrangement>()
                };
            }
            containerList.Add(container);
            PlacingBoxAdvance(storeName, containerList, container, 0, container.Width, container.Depth, container.Height, 0, sortedBoxes);
            while (GetWaitingBoxes.Count > 0)
            {
                var leftBox = GetWaitingBoxes;
                GetWaitingBoxes = new List<ProductShippingDimension>();
                var thisContainer = FindSuitableContainer(storeName, leftBox);
                containerList.Add(thisContainer);
                PlacingBoxAdvance(storeName, containerList, thisContainer, 0, container.Width, container.Depth, container.Height, 0, leftBox);
            }
            return containerList;
        }

        private Container FindSuitableContainer(string storeName, List<ProductShippingDimension> boxes)
        {
            if (boxes.Count == 0) return null;
            var lengthArray = GetDescendingSortedArray(RedimensionBoxes(boxes));
            var firstLongestEdge = lengthArray[0];
            var secondLongestEdge = lengthArray[1];
            var maxHeightEdge = boxes.Max(x => x.Height);
            var containers = new List<Container>();
            if (this._addExtraLength)
                containers = GetAvailableContainers(storeName)
                .Where(x => x.Width >= (firstLongestEdge + 1) && x.Depth >= (secondLongestEdge + 1) && x.Height >= (maxHeightEdge + 1)).ToList();
            else
                containers = GetAvailableContainers(storeName)
                .Where(x => x.Width >= firstLongestEdge && x.Depth >= secondLongestEdge && x.Height >= maxHeightEdge).ToList();
            var container = containers
                .OrderByDescending(x => x.Height)
                .OrderBy(x => x.Depth)
                .OrderBy(x => x.Width)
                .FirstOrDefault();

            if (container == null)
            {
                return null;
            }
            var thisContainer = new Container
            {
                ID = container.ID,
                ContainerName = container.ContainerName,
                PartNo = container.PartNo,
                Width = container.Width,
                Depth = container.Depth,
                Height = container.Height,
                Pounds = 0,
                Ounces = 0,
            };
            thisContainer.Arrangements = new List<Arrangement>();
            return thisContainer;
        }

        private List<ProductShippingDimension> RedimensionBoxes(List<ProductShippingDimension> boxes)
        {
            foreach (var b in boxes)
            {
                var lengths = new List<decimal> { b.Width, b.Depth, b.Height };
                var sortedLengths = lengths.OrderBy(x => x).ToArray();
                b.Width = sortedLengths[2];
                b.Depth = sortedLengths[1];
                b.Height = sortedLengths[0];
            }
            return boxes;
        }

        private decimal[] GetDescendingSortedArray(List<ProductShippingDimension> boxes)
        {
            List<decimal> lengthList = new List<decimal>();
            foreach (var b in boxes)
            {
                lengthList.Add(b.Width);
                lengthList.Add(b.Height);
                lengthList.Add(b.Depth);
            }
            var lengthArray = lengthList.ToArray();
            Array.Sort(lengthArray);
            lengthArray = lengthArray.Reverse().ToArray();
            return lengthArray;
        }

        private void PlacingBoxAdvance(string storeName, List<Container> containerList, Container container,
            int level, decimal widthLeft, decimal depthLeft, decimal heightLeft, decimal boxHighestHeight, List<ProductShippingDimension> leftBox)
        {
            if (leftBox.Count > 0)
            {
                if (leftBox[0].Width <= widthLeft && leftBox[0].Depth <= depthLeft)
                {
                    if (heightLeft >= leftBox[0].Height)
                    {
                        widthLeft = widthLeft - leftBox[0].Width;
                        depthLeft = depthLeft - leftBox[0].Depth;
                        boxHighestHeight = Math.Max(boxHighestHeight, leftBox[0].Height);

                        container.Arrangements.Add(new Arrangement { ProductShippingDimension = leftBox[0], Level = level });
                        leftBox.RemoveAt(0);

                        if (leftBox.Count > 0)
                            PlacingBoxAdvance(storeName, containerList, container, level, widthLeft, depthLeft, heightLeft, boxHighestHeight, leftBox);
                    }
                    else
                    {
                        // check if there is a next                        
                        if (leftBox.Count >= 1)
                        {
                            GetWaitingBoxes.Add(leftBox[0]);
                            leftBox.RemoveAt(0);
                            PlacingBoxAdvance(storeName, containerList, container, level, widthLeft, depthLeft, heightLeft, boxHighestHeight, leftBox);
                        }
                        else
                        {
                            container = FindSuitableContainer(storeName, GetWaitingBoxes);
                            /// No suitable container found, treate first box as a container
                            if (container == null)
                            {
                                container = new Container
                                {
                                    ID = leftBox[0].ID,
                                    PartNo = leftBox[0].PartNo,
                                    Width = leftBox[0].Width,
                                    Height = leftBox[0].Height,
                                    Depth = leftBox[0].Depth,
                                    Arrangements = new List<Arrangement>()
                                };
                            }
                            containerList.Add(container);
                            // placing box
                            level = 0;
                            boxHighestHeight = 0;
                            PlacingBoxAdvance(storeName, containerList, container, level, container.Width, container.Depth, container.Height, boxHighestHeight, GetWaitingBoxes);
                        }
                    }
                }
                else
                {
                    level += 1;
                    heightLeft = heightLeft - boxHighestHeight;
                    boxHighestHeight = 0;
                    if (leftBox.Count > 0)
                    {
                        PlacingBoxAdvance(storeName, containerList, container, level, container.Width, container.Depth, heightLeft, boxHighestHeight, leftBox);
                    }
                }
            }
        }
    }
}
