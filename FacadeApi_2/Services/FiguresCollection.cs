using FacadeCreatorApi.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi.Services
{
    public interface FiguresCollection: IEnumerable<FigureOnBoard>
    {
        bool add(FigureOnBoard addingFigure);
        bool insert(int index, FigureOnBoard insetingFigure);
        bool remove(FigureOnBoard removingFigure);
        bool remove(int index);
        bool removeAll();
        bool move(int from, int to);
        bool shuffle(int firstIndex, int secondIndex);

        int getIndex(FigureOnBoard figure);
        int getCount();
        FigureOnBoard get(int index);

        IEnumerable<FigureOnBoard> getReverseCollection();



    }
}
