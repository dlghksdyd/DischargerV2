#-*- coding:euc-kr -*-


from textwrap import indent
from xml.dom.minidom import Element
import xml.etree.ElementTree as ET
import os

def create_resimage_xaml():
    in_folder_path = "../../Image/"
    xaml_out_folder_path = "../../StaticResources/"

    # get image file name lists
    file_name_list = list()
    for file_name in os.listdir(in_folder_path):
        file_name_list.append(file_name)

    # xaml 파일 생성
    fp = open(xaml_out_folder_path + "ResImage.xaml", "w", encoding='UTF-8')
    fp.write("<!-- 본 코드는 자동 생성된 코드입니다. 수정 하지 말아 주세요. -->\n")
    fp.write("<!-- 본 코드는 자동 생성된 코드입니다. 수정 하지 말아 주세요. -->\n")
    fp.write("<!-- 본 코드는 자동 생성된 코드입니다. 수정 하지 말아 주세요. -->\n")
    fp.write("<!-- 본 코드는 자동 생성된 코드입니다. 수정 하지 말아 주세요. -->\n")
    fp.write("<!-- 본 코드는 자동 생성된 코드입니다. 수정 하지 말아 주세요. -->\n")
    fp.write("<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\n")
    fp.write("xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n\n")
    for one_file_name in file_name_list:
        extension = "." + one_file_name.split(".")[-1]
        fp.write("\t<BitmapImage x:Key=\"Mex/ResImage/" + one_file_name.replace(extension, "") + "\" UriSource=\"/MExpress;component/Image/" + one_file_name + "\"></BitmapImage>\n");
    fp.write("</ResourceDictionary>")
    fp.close()

def create_resimageview_resx():
    in_folder_path = "../../Image/"
    xaml_out_folder_path = "../../Mex/"

    # get image file name lists
    file_name_list = list()
    for file_name in os.listdir(in_folder_path):
        file_name_list.append(file_name)

    import xml.dom.minidom

    dom = xml.dom.minidom.parse(xaml_out_folder_path + "ResTemplate.resx")

    root = dom.documentElement
    
    for file_name in file_name_list:
        data_element = dom.createElement("data")
        data_element.setAttribute("name", file_name.replace(".png", ""))
        data_element.setAttribute("type", "System.Resources.ResXFileRef, System.Windows.Forms")
        value_element = dom.createElement("value")
        value_element_text = dom.createTextNode("..\\Image\\" + file_name + ";System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
        value_element.appendChild(value_element_text)
        data_element.appendChild(value_element)
        root.appendChild(data_element)

    fp = open(xaml_out_folder_path + "ResImageView.resx", "w", encoding='UTF-8')
    dom.writexml(fp, addindent='    ', newl='\n')
    fp.close()


def create_resources():
    # 유저가 정의한 리소스 타입 리스트를 가져온다.
    user_defined_type_list = list()
    folder_path = "../../StaticResources/"
    file_name = "ResUserDefinedType.cs"
    fp = open(folder_path + file_name, "r")
    for one_line in fp.readlines():
        if one_line.find("public class") != -1:
            user_defined_type_list.append(one_line.split(" ")[-1].replace("\n", ""))

    key_prefix = "{http://schemas.microsoft.com/winfx/2006/xaml}"

    #in_folder_path = "./"
    in_folder_path = "../../StaticResources/"

    #out_folder_path = "./"
    out_folder_path = "../../Mex/"

    # get .xaml file
    file_name_list = list()
    for file_name in os.listdir(in_folder_path):
        if file_name[-5:] == ".xaml":
            file_name_list.append(file_name)

    for file_name in file_name_list:
        file_name_without_extension = file_name.split(".")[0]

        tree = ET.parse(in_folder_path + file_name)

        class_name_dict = dict()

        object_list = list()

        root = tree.getroot()
        for child in root:
            child.tag = child.tag.split("}")[1]
            if (child.tag == "ResourceDictionary.MergedDictionaries"):
                continue

            # get key value
            key_property = child.attrib[key_prefix + "Key"]
            key_property = key_property.split("/")
            class_name_dict[key_property[1]] = key_property[1]

            one_object = OneObject()
            one_object.class_name = key_property[1]
            one_object.var_name = key_property[2]
            one_object.child_var_value = None

            if child.tag == "Double":
                one_object.var_type = "public static " + child.tag
                one_object.var_value = child.text + ";"
            elif child.tag == "FontWeight":
                one_object.var_type = "public static " + child.tag
                one_object.var_value = "FontWeights." + child.text + ";"
            elif child.tag == "FontFamily":
                one_object.var_type = "public static " + child.tag
                one_object.var_value = "new FontFamily(\"" + child.text + "\");"
            elif child.tag == "Color":
                one_object.var_type = "public static " + child.tag
                one_object.var_value = "Color.FromArgb("
                one_object.var_value += "0x" + child.text[1:3]
                one_object.var_value += ", 0x" + child.text[3:5]
                one_object.var_value += ", 0x" + child.text[5:7]
                one_object.var_value += ", 0x" + child.text[7:9]
                one_object.var_value += ");"
            elif child.tag == "StaticResource":
                if one_object.class_name == "ResColorAlias":
                    one_object.var_type = "public static Color"
                    resource_key_property = child.attrib["ResourceKey"]
                    resource_key_property = resource_key_property.split("/")
                    one_object.var_value = resource_key_property[1] + "." + resource_key_property[2] + ";"
            elif child.tag == "BitmapImage":
                one_object.var_type = "public static BitmapSource"
                one_object.var_value = "ResImageView." + one_object.var_name + ".GetBitmapSource();"
            elif child.tag == "SolidColorBrush":
                one_object.var_type = "public static " + child.tag
                one_object.var_value = "new " + child.tag + "()"
                one_object.child_var_value = "\t\t{\n"
                for one_attrib_key in child.attrib.keys():
                    if one_attrib_key[-3:] != "Key":
                        child_property = child.attrib[one_attrib_key]
                        child_property = child_property.split("{x:Static Mex:")[1].replace("}", "").replace("/", ".")
                        one_object.child_var_value += "\t\t\t" + one_attrib_key + " = " + child_property + ",\n"
                one_object.child_var_value += "\t\t};\n\n"
            elif child.tag == "ImageSet_Component" \
                    or child.tag == "ImageSet_Toggle":
                one_object.var_type = "public static " + child.tag
                one_object.var_value = "new " + child.tag + "()"
                one_object.child_var_value = "\t\t{\n"
                for one_attrib_key in child.attrib.keys():
                    if one_attrib_key[-3:] != "Key":
                        child_property = child.attrib[one_attrib_key]
                        image = child_property.split(",")[0].split("Mex/")[1].replace("/", ".").replace("}", "")
                        param = child_property.split(",")[2].split("Mex/")[1].replace("/", ".").replace("}}", "")
                        one_object.child_var_value += "\t\t\t" + one_attrib_key + " = "
                        one_object.child_var_value += "(BitmapImage) new ImageColorConverter().Convert("
                        one_object.child_var_value += image + ", null, "
                        one_object.child_var_value += param + ", null),\n"
                one_object.child_var_value += "\t\t};\n\n"
            elif child.tag == "Thickness" \
                    or child.tag == "CornerRadius" \
                    or child.tag == "SolidColorBrush" \
                    or child.tag in user_defined_type_list:
                one_object.var_type = "public static " + child.tag
                one_object.var_value = "new " + child.tag + "()"
                one_object.child_var_value = "\t\t{\n"
                for one_attrib_key in child.attrib.keys():
                    if one_attrib_key[-3:] != "Key":
                        child_property = child.attrib[one_attrib_key]
                        child_property = child_property.split("{StaticResource Mex/")[1].replace("}", "").replace("/", ".")
                        one_object.child_var_value += "\t\t\t" + one_attrib_key + " = " + child_property + ",\n"
                one_object.child_var_value += "\t\t};\n\n"

            object_list.append(one_object)

        f = open(out_folder_path + file_name_without_extension + ".cs", 'w')

        f.write("// 본 코드는 자동 생성된 코드입니다. 수정 하지 말아 주세요.\n")
        f.write("// 본 코드는 자동 생성된 코드입니다. 수정 하지 말아 주세요.\n")
        f.write("// 본 코드는 자동 생성된 코드입니다. 수정 하지 말아 주세요.\n")
        f.write("// 본 코드는 자동 생성된 코드입니다. 수정 하지 말아 주세요.\n")
        f.write("// 본 코드는 자동 생성된 코드입니다. 수정 하지 말아 주세요.\n")
        f.write("using System;\n")
        f.write("using System.Windows;\n")
        f.write("using System.Windows.Media.Imaging;\n")
        f.write("using System.Windows.Media;\n\n")
        f.write("namespace MExpress.Mex\n")
        f.write("{\n")

        for one_class_name in class_name_dict.keys():
            filtered_object_list = list(filter(lambda x: x.class_name == one_class_name, object_list))

            f.writelines("\tpublic class " + one_class_name + "\n")
            f.writelines("\t{\n")

            print(one_class_name)
            for item in filtered_object_list:
                f.writelines("\t\t" + item.var_type + " " + item.var_name + " = " + item.var_value + "\n")
                if item.child_var_value is not None:
                    f.writelines(item.child_var_value)

            f.writelines("\t}\n")

        f.writelines("}\n")
        f.close()

class OneObject:
    class_name: str
    var_name: str
    var_type: str
    var_value: str
    child_var_value: str

if __name__ == "__main__":
    create_resimageview_resx()
    create_resimage_xaml()

    create_resources()
