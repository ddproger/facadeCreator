using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi.models
{
    public enum TypeOfIterator
    {
        DRAW,
        OPERATE_WITH_FIGURE
    }
    public class FiguresCollection
    {
        LinkedList<Figure> bkgImage;
        LinkedList<Figure> facades;
        bool enableOnlyImages = false;
        public FiguresCollection()
        {
            bkgImage = new LinkedList<Figure>();
            facades = new LinkedList<Figure>();
        }
        public bool addFigure(Figure figure)
        {
            try {
                if (figure is BkgImage) bkgImage.AddLast(figure);
                else if (figure is Facade) facades.AddLast(figure);
                else throw new ArgumentException("We cannot recognize type of current object: "+ figure.GetType());
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public bool deleteFigure(Figure currentFigure)
        {
            if (currentFigure is BkgImage) bkgImage.RemoveLast();
            return true;
        }
        public IEnumerator<Figure> getItterator(TypeOfIterator type)
        {
            switch (type)
            {
                case TypeOfIterator.DRAW: return new MultipleCollectionsIterator<Figure>(new LinkedList<Figure>[] { bkgImage, facades });
                case TypeOfIterator.OPERATE_WITH_FIGURE:
                    if (enableOnlyImages)
                    {
                        return new MultipleCollectionsIterator<Figure>(new LinkedList<Figure>[] 
                                                                        {
                                                                            (LinkedList<Figure>)bkgImage.Reverse<Figure>()
                                                                        });
                    }
                    break;
            }
            return new MultipleCollectionsIterator<Figure>(new LinkedList<Figure>[]
                                                                        {
                                                                            (LinkedList<Figure>)facades.Reverse<Figure>(),
                                                                            (LinkedList<Figure>)bkgImage.Reverse<Figure>()
                                                                        });
        } 
    }

    internal class MultipleCollectionsIterator<Figure>: IEnumerator<Figure>
    {
        private LinkedList<Figure>[] figuresCollections;
        private Figure current;
        private int position;
        private int count;
        IEnumerator iter;

        public MultipleCollectionsIterator(LinkedList<Figure>[] figures)
        {
            figuresCollections = figures;
            count = figures.Length;
            Reset();
        }
        public Figure Current => current;

       
        object IEnumerator.Current => Current;

        public void Dispose()
        {
            iter = figuresCollections[0].GetEnumerator();
            position = 0;
        }

        public bool MoveNext()
        {
            if (iter.MoveNext())
            {
                current = (Figure)iter.Current;
                return true;                
            }
            else
            {
                if (position < count - 1)
                {
                    iter = figuresCollections[++position].GetEnumerator();
                    return MoveNext();
                }
            }
            return false;
        }

        public void Reset()
        {
            iter = figuresCollections[0].GetEnumerator();
            position = 0;
        }
    }
}
