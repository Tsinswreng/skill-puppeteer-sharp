---

name: tsinswreng-shell

description: shell命令使用規範。此skill必讀

---

## 在windows中也應優先用bash, 少用pwsh/cmd

僅在必要時候纔使用pwsh/cmd, 如調用Windows API。 本機上的`bash`即已指向git bash。

## 避免轉義與命令嵌套

- 多用heredoc來避免轉義, 少用引號字符串字面量。
- 若某命令支持從stdin讀輸入就優先用stdin配heredoc 凡是能用stdin/heredoc的 就絕對禁止`bash -c ''`式的字符串！ 避免以下常見錯誤做法:
  - ❌`sh -c 'some command'`
  - ❌`bash -c 'some command'`
  - ❌`su -c 'some command'`
  - ❌`eval "some command"`
  - ❌`ssh user@ip "some command"`
  - …正確示例:
  ```sh
  bash <<'EOF'
  some command
  EOF
  ```
- 若不支持從stdin讀輸入就用`"$(cat <<'EOF'...)"` 例(bash是支持從stdin讀的 這裏只是爲了舉例):
  ```sh
  bash -c "$(cat <<'EOF'
  some command
  EOF
  )"
  ```
- 注意: 純raw string的heredoc中 第一個定界符要加引號, 即寫成`<<'EOF'`。 因爲不加引號的`<<EOF`內
  - `$var`,`${var}`會變量展開
  - `$(cmd)`會命令替換
  - `\`會轉義

### 綜合示例

錯誤示例

```sh
wsl -u root -- bash -c 'curl -s -X POST https://httpbin.org/post -H "Content-Type: application/json" -d "{\"username\":\"admin\",\"password\":\"'\''\$114514\"}" -w "\nHTTP %{http_code}\n"'
```

正確示例

```sh
wsl -u root -- bash <<'EOF'
curl -s -X POST https://httpbin.org/post \
-H "Content-Type: application/json" \
-d "$(cat <<'EOF2'
{"username":"admin","password":"'$114514"}
EOF2
)" \
-w "\nHTTP %{http_code}\n"
EOF
```

## 複雜腳本用CsSh寫

需要寫臨時的複雜腳本時用CsSh寫 .csx文件, 用dotnet-script執行。 不用pwsh/bash等來寫。 bash/pwsh能做的事情 CsSh全都能做。 具體寫法看tsinswreng-cssh skill。 注意應按需加載skill, 當你不需要寫時就不要浪費時間讀無關skill。

當腳本代碼行數超過16行你就應該考慮用CsSh寫。

## 再次強調

- **絕對禁止** `bash -c ''` / `su -c`之類的寫法! 一律用stdin配heredoc
- 複雜腳本用CsSh寫。當腳本代碼行數超過16行你就應該考慮用CsSh寫。
