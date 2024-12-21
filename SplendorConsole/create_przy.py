import json
import numpy as np


import math


def standardize(array):
    n = len(array)
    sum_values = sum(array)

    mean = sum_values / n

    q = 0
    for i in range(n):
        xiu = (array[i] - mean) ** 2
        q += xiu

    standard_deviation = math.sqrt(q / n)

    final_z_score = np.zeros(n, dtype=np.float32)

    for i in range(n):
        z_score = (array[i] - mean) / standard_deviation
        final_z_score[i] = z_score

    return final_z_score.tolist()  # Convert the NumPy array to a regular Python list before returning

# Example usage:
array = [1, 2, 3, 4, 5]
z_scores = standardize(array)
print(z_scores)


# Generate the array of 348 random integers in the range [0, 30]
random_array = np.random.randint(0, 31, size=348).tolist()
random_array = standardize(random_array)
# Generate the additional random integer
random_int = np.random.randint(0, 31)

# Create the JSON structure
example_json = {
    "state": random_array,
    "reward": random_int
}

# Convert to JSON string for display
json_output = json.dumps(example_json, indent=4)
print(json_output)
