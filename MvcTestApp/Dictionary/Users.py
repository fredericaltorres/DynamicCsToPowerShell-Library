class User(object):

    def __init__(self, userName):
        self.UserName = userName
        self._toStringCounter = 0

    def ToString(self):
        self._toStringCounter += 1
        return "user:%s %s" % (self.UserName, self._toStringCounter)

Dic = {
    "Users" : [ 
                    User("FTorres"), 
                    User("RDescartes")
    ]
}


