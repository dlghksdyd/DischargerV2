import os
import shutil

def resolve_path(*parts):
    """Resolve absolute path based on script location"""
    base_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
    return os.path.join(base_dir, *parts)

def copy_config_files(source_dir, target_dir):
    """Copy files from Config folder to bin\Release\Database"""
    if not os.path.isdir(source_dir):
        print(f"[ERROR] Source directory not found: {source_dir}")
        return

    os.makedirs(target_dir, exist_ok=True)

    copied = 0
    for name in os.listdir(source_dir):
        src = os.path.join(source_dir, name)
        dst = os.path.join(target_dir, name)

        if os.path.isfile(src):
            try:
                shutil.copy2(src, dst)
                print(f"Copied: {name}")
                copied += 1
            except Exception as e:
                print(f"[ERROR] Failed to copy {name}: {e}")

    if copied == 0:
        print("No files copied.")
    else:
        print(f"Total files copied: {copied}")

def main():
    source = resolve_path("Config")
    target = resolve_path("bin", "Release", "Database")
    copy_config_files(source, target)

if __name__ == "__main__":
    main()