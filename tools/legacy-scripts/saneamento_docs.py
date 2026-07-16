import re
import os

DOC = 'docs/roadmap/sprint-7-gerenciamento-requisicoes.md'

with open(DOC, 'r', encoding='utf-8') as f:
    c = f.read()

# Make sure 10, 13, 14 have [x] instead of [ ]
c = re.sub(r'- \[ \] 10\.', r'- [x] 10.', c)
c = re.sub(r'- \[ \] 13\.', r'- [x] 13.', c)
c = re.sub(r'- \[ \] 14\.', r'- [x] 14.', c)

# And if they were listed without brackets at all:
# wait, item 10 didn't have brackets in my previous cat! 
# Let me just forcefully replace '10. Aplicar' with '- [x] 10. Aplicar'
c = re.sub(r'^10\. Aplicar', r'- [x] 10. Aplicar', c, flags=re.MULTILINE)
c = re.sub(r'^13\. Implementar', r'- [x] 13. Implementar', c, flags=re.MULTILINE)
c = re.sub(r'^14\. Validar', r'- [x] 14. Validar', c, flags=re.MULTILINE)

# Ensure 37, 38, 39 are [ ]
c = re.sub(r'- \[x\] 37\.', r'- [ ] 37.', c)
c = re.sub(r'- \[x\] 38\.', r'- [ ] 38.', c)
c = re.sub(r'- \[x\] 39\.', r'- [ ] 39.', c)

# Make sure percentage is 92% and count is 36/39
c = re.sub(r'\d+% \(', r'92% (', c)
c = re.sub(r'\d+/\d+', r'36/39', c)

with open(DOC, 'w', encoding='utf-8') as f:
    f.write(c)
