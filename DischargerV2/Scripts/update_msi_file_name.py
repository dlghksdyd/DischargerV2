import os
import re
import shutil
import sys

sys.stdout.reconfigure(encoding='utf-8')

# === 상대 경로 설정 ===
BASE_DIR = os.path.dirname(os.path.abspath(__file__))
ASSEMBLY_INFO_PATH = os.path.join(BASE_DIR, "..", "Properties", "AssemblyInfo.cs")
MSI_OUTPUT_DIR = os.path.join(BASE_DIR, "..", "..", "Smart Discharger", "Release")
MSI_FILE_NAME = "Smart Discharger"

# AssemblyInformationalVersion 정규식 (1.2.3 또는 1.2.3-beta 허용)
RE_ASSEMBLY_INFO_VERSION = re.compile(
    r'\[assembly:\s*AssemblyInformationalVersion\("([0-9]+\.[0-9]+\.[0-9]+)(?:[^\"]*)?"\)\]'
)


def read_file(path):
    if not os.path.exists(path):
        print(f"File not found: {path}")
        return None
    with open(path, 'r', encoding='utf-8') as f:
        return f.readlines()


def extract_informational_version(path):
    lines = read_file(path)
    if not lines:
        return None
    match = RE_ASSEMBLY_INFO_VERSION.search(''.join(lines))
    if match:
        version = match.group(1)  # '1.2.3' 추출
        print(f"Found AssemblyInformationalVersion: {version}")
        return version
    print("AssemblyInformationalVersion not found in file.")
    return None


def rename_msi(msi_dir, base_name, version):
    old_path = os.path.join(msi_dir, f"{base_name}.msi")
    new_name = f"{base_name}_{version}.msi"
    new_path = os.path.join(msi_dir, new_name)

    if not os.path.exists(old_path):
        print(f"MSI file not found: {old_path}")
        return

    shutil.move(old_path, new_path)
    print(f"MSI file renamed to: {new_path}")


def main():
    version = extract_informational_version(ASSEMBLY_INFO_PATH)
    if version:
        rename_msi(MSI_OUTPUT_DIR, MSI_FILE_NAME, version)


if __name__ == '__main__':
    main()
