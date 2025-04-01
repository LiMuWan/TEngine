# -*- coding: utf-8 -*-
from MsgID.csfile import genCSfile
from MsgID.proto import loadProto
import sys

protos = loadProto()

op = sys.argv[1]
print("arg = ", op)
if op=="client":
	print("gen cs files...")
	genCSfile(protos)





