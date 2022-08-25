using System.Collections.Generic;

namespace PathService
{
    class PathFinder
    {
        readonly int _n;
        readonly int[] _x = { 1, 1, -1, -1, 2, 2, -2, -2 };
        readonly int[] _y = { 2, -2, 2, -2, 1, -1, 1, -1 };
        readonly Queue<(int first, int second)> _q = new Queue<(int first, int second)>();
        readonly (int first, int second) _start, _end;
        (int first, int second) _temp;
        readonly (int first, int second) _nan = (first: -1, second: -1);
        readonly (int first, int second)[][] _p;

        private PathFinder()
        {
        }

        public PathFinder(string start, string end, int moves) : this()
        {
            _n = moves;
            _p = new (int first, int second)[_n][];

            for (int i = 0; i < _p.Length; i++)
            {
                _p[i] = new (int first, int second)[_n];
                for (int j = 0; j < _n; j++)
                {
                    _p[i][j] = (first: -1, second: -1);
                }
            }

            _start = (start[0] - 0x61 /* 'a' in ASCII */, start[1] - 0x31 /* '1' - in ASCII */);
            _end = (end[0] - 0x61, end[1] - 0x31);
        }

        public List<string> GetPath()
        {
            _q.Enqueue(_start);
            _p[_start.first][_start.second] = _start;

            while (_q.Count != 0)
            {
                _temp = _q.Dequeue();
                if (_temp == _end)
                {
                    List<(int first, int second)> way = new List<(int first, int second)>();
                    while (_temp != _start)
                    {
                        way.Add(_temp);
                        _temp = _p[_temp.first][_temp.second];
                    }
                    way.Add(_start);

                    List<string> wayResult = new List<string>(way.Count);
                    foreach (var t in way)
                    {
                        wayResult.Add(coordinateToString(t));
                    }

                    wayResult.Reverse();

                    return wayResult;
                }

                for (int i = 0; i < 8; i++)
                {
                    int nx = _temp.first + _x[i];
                    int ny = _temp.second + _y[i];
                    if (nx >= 0 && ny >= 0 && nx < _n && ny < _n && _p[nx][ny] == _nan)
                    {
                        _p[nx][ny] = _temp;
                        _q.Enqueue((first: nx, second: ny));
                    }
                }
            }

            List<string> res = new List<string> {"No way"};

            return res;
        }

        private string coordinateToString((int first, int second) coordinate)
        {
            char lx = (char)(coordinate.first + 0x61);
            char ly = (char)(coordinate.second + 0x31);

            string res = $"{lx}{ly}";

            return res;
        }

    }

}
