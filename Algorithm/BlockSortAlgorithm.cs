namespace RefExample2.Algorithm;

public class BlockSortAlgorithm
{
    private readonly int[] _array;
    private readonly int _n;
    private readonly int _blockSize;
    private readonly int _blockCount;

    public BlockSortAlgorithm(int[] input)
    {
        _array = input;
        _n = _array.Length;

        _blockSize = (int)Math.Sqrt(_n);
        
        if(_blockSize == 0) _blockSize = 1;

        _blockCount = (_n + _blockSize - 1) / _blockSize;
    }

    public void Sort()
    {
        for (var i = 0; i < _blockCount; i++)
        {
            var left = i * _blockSize;
            var right = Math.Min(left + _blockSize, _n);  
            
            InsertionSort(_array, left, right);
        }

        var buffer = new int[_n];
        var size = _blockSize;

        while (size < _n)
        {
            for (var left = 0; left < _n; left += 2 * size)
            {
                var mid = Math.Min(left + size, _n);
                var right = Math.Min(left + 2 * size, _n);

                Merge(_array, buffer, left, mid, right);
            }

            size *= 2;
        }
    }

    private static void InsertionSort(int[] arr, int left, int right)
    {
        for (var i = left + 1; i < right; i++)
        {
            var key = arr[i];
            var j = i - 1;
            while (j >= left && arr[j] > key)
            {
                arr[j + 1] = arr[j];
                j--;
            }
            arr[j + 1] = key;
        }
    }

    private static void Merge(int[] arr, int[] buffer, int left, int mid, int right)
    {
        int i = left, j = mid, k = left;

        while (i < mid && j < right)
        {
            if (arr[i] <= arr[j])
                buffer[k++] = arr[i++];
            else
                buffer[k++] = arr[j++];
        }

        while (i < mid) buffer[k++] = arr[i++];
        while (j < right) buffer[k++] = arr[j++];

        for (i = left; i < right; i++)
            arr[i] = buffer[i];
    }
}