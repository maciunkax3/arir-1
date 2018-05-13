using System;
using PRAM_Machine.Machine;
using PRAM_Machine.Memory;

namespace PRAM_Machine.Samples.MatrixMultiplicationUsingMatrixDataType {
    class MatrixMultiplicationUsingMatrixDataTypeProcessor : Processor {
        private readonly int _row;
        private readonly int _column;
        private readonly int _size;
        
        private int ClusterNumber { get { return _row*_size + _column; } }
        private readonly int _num;

        private int _value;
        private int _sum;
        private int _index1;
        private int _index2;
        private int _val1;
        private int _val2;
        private int step;
        private int p;

        public MatrixMultiplicationUsingMatrixDataTypeProcessor(int row, int column, int size) : base() {
            this._row = row;
            this._column = column;
            this._size = size;
            this._sum = 0;
            this.step = 0;
            this.p = size/2;
            this.DataToRead = new MemoryAddress("MatrixA",_row,_column);
        }

        public override dynamic Run(dynamic data)
        {
            switch (step)
            {
                case 0:
                        step = 1;
                        this.DataToWrite = new MemoryAddress("MatrixA", _row, _column);
                    if (_row < p)
                        this.DataToRead = new MemoryAddress("MatrixA", 2 * _row, _column);
                    else
                        Stop();
                    return ((data + 1) % 2);
                case 1:
                    if (TickCount % 2 == 1)
                    {
                        _sum = data;
                        this.DataToRead = new MemoryAddress("MatrixA", 2 * _row + 1, _column);
                        this.DataToWrite = new MemoryAddress();
                        return null;
                    }
                    else
                    {
                        _sum += data;
                        this.DataToWrite = new MemoryAddress("MatrixA", _row, _column);
                        p /= 2;
                        if (p == 0)
                        {
                            step = 2;
                            p = _size / 2;
                            if (_column < p)
                                this.DataToRead = new MemoryAddress("MatrixB", 0, 2 * _column);
                            else
                                Stop();
                            return _sum;
                        }
                        if (_row < p)
                            this.DataToRead = new MemoryAddress("MatrixA", 2 * _row, _column);
                        else
                            Stop();

                        return _sum;
                    }
                case 2:
                    if (TickCount % 2 == 1)
                    {
                        _index1 = data;
                        this.DataToRead = new MemoryAddress("MatrixB", 0, 2 * _column+1);
                        this.DataToWrite = new MemoryAddress();
                        return null;
                    }
                    else
                    {
                        _index2 = data;
                        this.DataToRead = new MemoryAddress("MatrixA", 0, _index1);
                        this.DataToWrite = new MemoryAddress();
                        step = 3;
                        return null;
                    }
                case 3:
                    if (TickCount % 2 == 1)
                    {
                        _val1 = data;
                        this.DataToRead = new MemoryAddress("MatrixA", 0, _index2);
                        this.DataToWrite = new MemoryAddress();
                        return null;
                    }
                    else
                    {

                        _val2 = data;
                        this.DataToWrite = new MemoryAddress("MatrixB", 0,_column);
                        var index = (_val1 > _val2 ? _index1 : _index2);
                        p = p / 2;
                        if (p == 0)
                        {
                            this.DataToWrite = new MemoryAddress("MatrixB", 0, 0);
                            Stop();
                            return index;
                        }
                        if (_column < p)
                        {
                            this.DataToRead = new MemoryAddress("MatrixB", 0, 2 * _column);
                        }
                        else
                        {
                            Stop();
                        }
                        step = 2;
                        return index;
                    }
            }
            return 7;
            
            /*
            if (TickCount < 2)
            {
                if (TickCount == 0)
                {
                    _value = data;
                    DataToRead = new MemoryAddress("MatrixB", _num,_column);
                    return null;
                }
                DataToWrite = new MemoryAddress("temp" + ClusterNumber.ToString(), _num);
                DataToRead = new MemoryAddress("temp" + ClusterNumber.ToString(), _num);
                return _value * data;
            }
            if (_num + (int)Math.Pow(2, TickCount - 3) < _size)
            {
                _sum += data;
            }
            // If first processor should work
            if ((int)Math.Pow(2, TickCount - 2) < _size)
            {
                // If any other processor should work
                if (_num % (int)Math.Pow(2, TickCount - 1) == 0)
                {
                    if (_num + (int)Math.Pow(2, TickCount - 2) < _size)
                    {
                        this.DataToRead = new MemoryAddress("temp" + ClusterNumber.ToString(), 
                            _num + (int)Math.Pow(2, TickCount - 2));
                    }
                    else
                    {
                        this.DataToRead = new MemoryAddress();
                    }
                    this.DataToWrite = new MemoryAddress("temp" + ClusterNumber.ToString(), _num);
                    return _sum;
                }
                Stop();
                this.DataToWrite = new MemoryAddress();
                return _sum;
            }
            if (_num == 0)
            {
                DataToWrite = new MemoryAddress("MatrixC", _row,_column);
            }
            Stop();
            return _sum;
            */
        }
    }
}