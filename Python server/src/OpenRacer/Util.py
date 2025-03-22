import pkg_resources

resource_package = __name__

def loadFile(path):
    return pkg_resources.resource_string(resource_package, path)    
