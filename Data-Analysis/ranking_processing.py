import csv
from typing import Callable


def default_func(list: list[int]) -> float:
    r = 0.0
    list.sort()
    pts = [10 / (e + 9) for e in list]
    for i, pt in enumerate(pts):
        r += (0.5 ** (i + 1)) * pt
    return r * 9 + 1


def process_ranking(path: str, weight_func: Callable[[list[int]], float] = default_func) -> dict[str, float]:
    """
    Returns dict of [Title, Weight]
    :param path: The path of /data/
    :param weight_func: Function(e: list[int]) -> float to calculate the weight of music by ranks in history.
    """
    f = open(path + '/ranking_rearrange.csv', 'r', encoding='utf-8')
    r = dict[str, float]()
    with f:
        reader = csv.reader(f)
        # [Title] , [Month]:[Rank] , ...
        for row in reader:
            title = row[0]
            data = row[1:]
            ranks = [int(e.split(':')[1]) for e in data]
            r[title] = weight_func(ranks)

    r = {k: v for k, v in sorted(r.items(), key=lambda item: item[1], reverse=True)}
    for k, v in r.items():
        print(k, v)

    return r
