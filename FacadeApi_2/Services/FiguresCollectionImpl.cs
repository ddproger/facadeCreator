using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacadeCreatorApi.models;
using System.Runtime.InteropServices;

namespace FacadeCreatorApi.Services
{
    [ComVisible(false)]
    class FiguresCollectionImpl : FiguresCollection
    {
        private ArrayList mas;
        public FiguresCollectionImpl()
        {
            mas = new ArrayList();
        }
        public bool add(FigureOnBoard addingFigure)
        {
            if (mas.Add(addingFigure) != 0) return true;
            else return false;
        }

        public FigureOnBoard get(int index)
        {
            return (FigureOnBoard)mas[index];
        }

        public int getCount()
        {
            return mas.Count;
        }

        public IEnumerator<FigureOnBoard> GetEnumerator()
        {
            return new Enumerator(this);
        }

        public int getIndex(FigureOnBoard figure)
        {
            return mas.IndexOf(figure);
        }

        public IEnumerable<FigureOnBoard> getReverseCollection()
        {
            return new ReverseCollection(this);
        }

        public bool insert(int index,FigureOnBoard insetingFigure)
        {
            mas.Insert(index, insetingFigure);
            return true;
        }

        public bool move(int from, int to)
        {
            FigureOnBoard tmp = (FigureOnBoard)mas[from];
            mas.Remove(tmp);
            mas.Insert(to, tmp);
            return true;
        }

        public bool remove(FigureOnBoard removingFigure)
        {
            mas.Remove(removingFigure);
            return true;
        }

        public bool remove(int index)
        {
            remove(get(index));
            return true;
        }
        public bool removeAll()
        {
            mas.Clear();
            return true;
        }

        public bool shuffle(int firstIndex, int secondIndex)
        {
            FigureOnBoard tmp = (FigureOnBoard)mas[secondIndex];
            mas[secondIndex] = mas[firstIndex];
            mas[firstIndex] = tmp;
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    class Enumerator : IEnumerator<FigureOnBoard>
    {
        public FigureOnBoard Current => collection.get(index);

        object IEnumerator.Current => Current;
        FiguresCollection collection;
        int index = -1;
        public Enumerator(FiguresCollection collection)
        {
            this.collection = collection;
            this.index = -1;
        }
        public void Dispose()
        {
            index = -1;
        }

        public bool MoveNext()
        {
            if (++index >= collection.getCount()) return false;
            return true;
        }

        public void Reset()
        {
            index = -1;
        }
    }
    class ReverseCollection : IEnumerable<FigureOnBoard>
    {
        FiguresCollection col;
        public ReverseCollection(FiguresCollection col)
        {
            this.col = col;
        }
        public IEnumerator<FigureOnBoard> GetEnumerator()
        {
            return new ReverseEnumerator(col);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    class ReverseEnumerator : IEnumerator<FigureOnBoard>
    {
        public FigureOnBoard Current => collection.get(index);

        object IEnumerator.Current => Current;
        FiguresCollection collection;
        int index = 0;
        public ReverseEnumerator(FiguresCollection collection)
        {
            this.collection = collection;
            this.index = collection.getCount();
        }
        public void Dispose()
        {
            collection.getCount();
        }

        public bool MoveNext()
        {
            if (--index < 0) return false;
            return true;
        }

        public void Reset()
        {
            index = collection.getCount();
        }
    }
}
