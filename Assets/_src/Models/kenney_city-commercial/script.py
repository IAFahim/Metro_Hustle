import os

if __name__ == '__main__':
    cwd = os.getcwd()
    for dirpath, dirnames, filenames in os.walk(cwd):
        for filename in filenames:
            if not filename.endswith(".meta"):
                continue
            filepath = os.path.join(dirpath, filename)
            content = ""
            with open(filepath, "r") as f:
                content = f.read()
                content = content.replace("externalObjects: {}", """externalObjects:
  - first:
      type: UnityEngine:Material
      assembly: UnityEngine.CoreModule
      name: colormap
    second: {fileID: 2100000, guid: bbeff65d43cf8b14fbfccdd0067a34f0, type: 2}""")
            with open(filepath, "w") as f:
                f.write(content)
