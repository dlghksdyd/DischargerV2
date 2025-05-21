import os
import re
import sys
import uuid

sys.stdout.reconfigure(encoding='utf-8')

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
ASSEMBLY_INFO_PATH = os.path.join(BASE_DIR, "..", "Properties", "AssemblyInfo.cs")
VDPROJ_PATH = os.path.join(BASE_DIR, "..", "..", "Mintech Discharger", "Mintech Discharger.vdproj")

RE_ASSEMBLY_INFO_VERSION = re.compile(r'\[assembly:\s*AssemblyInformationalVersion\("(\d+)\.(\d+)\.(\d+)(?:\.(\d+))?"\)\]')
RE_PRODUCT_VERSION_LINE = re.compile(r'"ProductVersion" = "8:([\d\.]+)"')
RE_PRODUCT_CODE_LINE = re.compile(r'"ProductCode" = "8:{([A-F0-9\-]+)}"', re.IGNORECASE)


def read_file(path):
    if not os.path.exists(path):
        print(f"File not found: {path}")
        return None
    with open(path, 'r', encoding='utf-8') as f:
        return f.readlines()


def write_file(path, lines):
    with open(path, 'w', encoding='utf-8') as f:
        f.writelines(lines)


def extract_assembly_version(path):
    lines = read_file(path)
    if not lines:
        return None
    match = RE_ASSEMBLY_INFO_VERSION.search(''.join(lines))
    if match:
        version_parts = match.groups()
        version = '.'.join(filter(None, version_parts))  # remove trailing None if 3-part version
        print(f"Found AssemblyInformationalVersion: {version}")
        return version
    print("AssemblyInformationalVersion not found in file.")
    return None


def update_vdproj(path, new_version):
    lines = read_file(path)
    if not lines:
        return

    current_version = None
    current_product_code = None
    new_version_short = '.'.join(new_version.split('.')[:3])

    for line in lines:
        if "ProductVersion" in line:
            match = RE_PRODUCT_VERSION_LINE.search(line)
            if match:
                current_version = match.group(1)
        if "ProductCode" in line:
            match = RE_PRODUCT_CODE_LINE.search(line)
            if match:
                current_product_code = match.group(1)

    if current_version == new_version_short:
        print(f"ProductVersion is already up to date: {current_version}")
        return

    print(f"Updating ProductVersion: {current_version} -> {new_version_short}")
    new_guid = str(uuid.uuid4()).upper()
    updated_lines = []

    for line in lines:
        if "ProductVersion" in line:
            line = RE_PRODUCT_VERSION_LINE.sub(f'"ProductVersion" = "8:{new_version_short}"', line)
        elif "ProductCode" in line:
            print(f"Replacing ProductCode: {current_product_code} -> {new_guid}")
            line = RE_PRODUCT_CODE_LINE.sub(f'"ProductCode" = "8:{{{new_guid}}}"', line)
        updated_lines.append(line)

    write_file(path, updated_lines)
    print(".vdproj file updated.")

    info_string = ""
    info_string += "Update complete. Please rebuild the project to apply changes.\n"
    info_string += "Update complete. Please rebuild the project to apply changes.\n"
    info_string += "Update complete. Please rebuild the project to apply changes.\n"
    info_string += "Update complete. Please rebuild the project to apply changes.\n"
    info_string += "Update complete. Please rebuild the project to apply changes.\n"
    print(info_string)

    sys.exit(1)


def main():
    version = extract_assembly_version(ASSEMBLY_INFO_PATH)
    if version:
        update_vdproj(VDPROJ_PATH, version)


if __name__ == '__main__':
    main()
