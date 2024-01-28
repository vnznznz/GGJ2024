import json
from collections import defaultdict

with open("Assets\Data\Actions.json", encoding="utf-8") as f:
    data = json.load(f)


audience_total_modif = defaultdict(lambda : 0)


for action in data["actions"]:
    print(action["text"])
    for result in action["result"]:
        print(f"\t{result['audience']}: {result['value']}")
        audience_total_modif[result["audience"]] += result["value"]

print(audience_total_modif)