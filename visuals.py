import matplotlib.pyplot as plt
import numpy as np
import time
import random

# -----------------------------
# 1) График асимптотик
# -----------------------------

n_values = np.linspace(100, 2000, 100, dtype=int)

bubble_complexity = n_values ** 2
blocksort_complexity = n_values * np.log2(n_values)
arraysort_complexity = n_values * np.log2(n_values)

plt.figure(figsize=(10, 6))
plt.plot(n_values, bubble_complexity / 1000, label="BubbleSort O(n²)")
plt.plot(n_values, blocksort_complexity, label="BlockSort O(n log n)")
plt.plot(n_values, arraysort_complexity, label="Array.Sort O(n log n)", linestyle="dashed")

plt.title("Асимптотики алгоритмов сортировки")
plt.xlabel("Размер массива (n)")
plt.ylabel("Относительная сложность (операции)")
plt.legend()
plt.grid(True)
plt.show()


# -----------------------------
# 2) График времязатрат
# -----------------------------

def bubble_sort(arr):
    arr = arr.copy()
    n = len(arr)
    for i in range(n - 1):
        for j in range(n - i - 1):
            if arr[j] > arr[j + 1]:
                arr[j], arr[j + 1] = arr[j + 1], arr[j]
    return arr

def block_sort(arr):
    # Простая модель: сортируем блоками вставками, потом слияние (на основе numpy sort для скорости)
    arr = arr.copy()
    n = len(arr)
    block_size = int(np.sqrt(n)) or 1
    blocks = [arr[i:i+block_size] for i in range(0, n, block_size)]
    for block in blocks:
        block.sort()
    # Сливаем блоки с помощью numpy.sort (приближение)
    return np.sort(np.concatenate(blocks))

def array_sort(arr):
    return sorted(arr)

sizes = [200, 400, 800, 1600]  # размеры массивов
times_bubble = []
times_block = []
times_array = []

for n in sizes:
    data = [random.randint(0, 10000) for _ in range(n)]

    start = time.time()
    bubble_sort(data)
    times_bubble.append(time.time() - start)

    start = time.time()
    block_sort(data)
    times_block.append(time.time() - start)

    start = time.time()
    array_sort(data)
    times_array.append(time.time() - start)

plt.figure(figsize=(10, 6))
plt.plot(sizes, times_bubble, label="BubbleSort")
plt.plot(sizes, times_block, label="BlockSort")
plt.plot(sizes, times_array, label="Array.Sort")
plt.title("Времязатраты алгоритмов сортировки")
plt.xlabel("Размер массива (n)")
plt.ylabel("Время (секунды)")
plt.legend()
plt.grid(True)
plt.show()
